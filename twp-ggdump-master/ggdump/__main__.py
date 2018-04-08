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

import argparse
import os
import pdb

import ggdump

def main():
    parser = argparse.ArgumentParser(
        prog='python -m ggdump',
        epilog='Example: python -m ggdump ThimbleweedPark.ggpack1 "*Sheet.png"'
    )
    parser.add_argument('ggpack_file', help='ggpack archive file path')
    parser.add_argument('search_pattern', help='file search pattern, wildcard ' + \
                        'patterns should be "quoted".')
    parser.add_argument('-w', help='write the files that match the pattern (otherwise ' + \
                        'only list files)', action='store_true')
    parser.add_argument('-v', '--verbose', help='output some debug information', \
                        action='store_true')

    args = parser.parse_args()
    if args.verbose:
        print("ggpack path: " + args.ggpack_file)
        print("search pattern: " + args.search_pattern)
        print("write files: " + str(args.w))

    if not os.path.isfile(args.ggpack_file):
        raise RuntimeError('File not found: ' + args.ggpack_file)

    gg_dir = ggdump.GGDirectory()
    gg_dir.gg_file_name = args.ggpack_file
    gg_dir.prepare_index()

    gg_dump = ggdump.GGDump()
    gg_dump.gg_file_name = args.ggpack_file
    gg_dump.gg_dir = gg_dir

    if args.w:
        gg_dump.dump_files(args.search_pattern)
    else:
        gg_dump.list_files(args.search_pattern)

if __name__ == "__main__":
    main()
