#!/usr/bin/python
# -*- coding: utf-8 -*-

# Thimbleweed park ggpack file decoder
# Copyright (C) 2017 mstr-

# This program is free software; you can redistribute it and/or
# modify it under the terms of the GNU General Public License
# as published by the Free Software Foundation; either version 2
# of the License, or (at your option) any later version.

# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.

# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

import struct
import fnmatch
import pdb

class GGDirectory:

    def __init__(self):
        self.file_index = []
        self.gg_file_name = ''

    def is_string_int(self, teststr):
        try:
            dummy = int(teststr)
        except ValueError:
            return False
        else:
            return True

    def read_c_string(self, buf, offset):
        cstr = bytearray()
        while offset < len(buf) and buf[offset] != 0x00:
            cstr.append(buf[offset])
            offset += 1
        return cstr.decode('utf-8')

    def read_gg_directory_to_buffer(self):
        f = open(self.gg_file_name, "rb")
        data_offset = struct.unpack('<i', f.read(4))[0]
        data_size = struct.unpack('<i', f.read(4))[0]
        f.seek(data_offset)
        buf = f.read(data_size)
        f.close()
        decoded_buf = decode_unbreakable_xor(buf)
        return decoded_buf

    def prepare_index(self):
        ibuf = self.read_gg_directory_to_buffer()

        fw = open('c:/users/ben/desktop/testfile', "wb")
        fw.write(ibuf)
        fw.close()


        # check directory signature
        if ibuf[0:4] != b'\x01\x02\x03\x04':
            raise RuntimeError('GGPack directory signature incorrect: ' + ibuf[0:4])
        # read ptr list offset & point to first file name offset
        plo = struct.unpack('<i', ibuf[8:12])[0]
        plo = plo + 1
        # convert index ptr list to list of fname, offset, size values
        index_entries = []
        while ibuf[plo:plo+4] != b'\xFF\xFF\xFF\xFF':
            offset = struct.unpack('<i', ibuf[plo:plo+4])[0]
            plo += 4
            entry = self.read_c_string(ibuf, offset)
            if entry in [b'files', b'filename', b'offset', b'size']:
                continue
            if self.is_string_int(entry):
                entry = int(entry)
            index_entries.append(entry)
        self.file_index = []
        i = 0
        # build file list of tuples: (filename, offset, size)
        # handle structural errors in index: size might be missing
        while i < len(index_entries) - 2:
            fname = index_entries[i]
            i += 1
            if not isinstance(index_entries[i], int):
                record = (fname, None, None)
                self.file_index.append(record)
                continue
            foffset = index_entries[i]
            i += 1
            if not isinstance(index_entries[i], int):
                record = (fname, foffset, None)
                self.file_index.append(record)
                continue
            fsize = index_entries[i]
            i += 1
            record = (fname, foffset, fsize)
            self.file_index.append(record)
        # TODO: try to fix empty size field

    def filter_index(self, pattern):
        return [elem for elem in self.file_index if fnmatch.fnmatchcase(elem[0], pattern)]

class GGDump:

    def __init__(self):
        self.gg_file_name = ''
        self.gg_dir = None

    def unpack_file_to_buf(self, offset, size):
        f = open(self.gg_file_name, "rb")
        f.seek(offset)
        buf = f.read(size)
        decoded_buf = decode_unbreakable_xor(buf)
        return decoded_buf

    def unpack_file_to_disk(self, output_file_name, offset, size):
        decoded_buf = self.unpack_file_to_buf(offset, size)
        fw = open(output_file_name, "wb")
        fw.write(decoded_buf)
        fw.close()

    def list_files(self, pattern):
        for rec in self.gg_dir.filter_index(pattern):
            print(rec[0])

    def dump_files(self, pattern):
        for rec in self.gg_dir.filter_index(pattern):
            if rec[1] is None or rec[2] is None:
                print('SKIPPED: (missing offset or size) ' + rec[0])
                continue
            self.unpack_file_to_disk(rec[0], rec[1], rec[2])
            print('Writing: ' + rec[0])

def decode_unbreakable_xor(src):
    magic_bytes = b'\x4F\xD0\xA0\xAC\x4A\x5B\xB9\xE5\x93\x79\x45\xA5\xC1\xCB\x31\x93'
    buffer = bytearray(src)
    buf_len = len(src)

    eax = buf_len
    var4 = buf_len & 0xFF
    ebx = 0
    while ebx < buf_len:
        eax = ebx & 0xFF
        eax = eax * -0x53   #6D
        ecx = ebx & 0x0F
        eax = (eax ^ magic_bytes[ecx]) & 0xFF
        ecx = var4
        eax = (eax ^ ecx) & 0xFF
        buffer[ebx] = buffer[ebx] ^ eax
        ecx = ecx ^ buffer[ebx]
        ebx = ebx + 1
        var4 = ecx

    i = 5
    while i < buf_len:
        buffer[i] = buffer[i] ^ 0x0D
        buffer[i+1] = buffer[i+1] ^ 0x0D
        i += 16

##    for i in range(5, buf_len - 5, 16):
##        buffer[i] = buffer[i] ^ 0x0D
##    for i in range(6, buf_len - 6, 16):
##        buffer[i] = buffer[i] ^ 0x0D

    return bytes(buffer)
