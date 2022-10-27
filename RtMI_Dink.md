# Scripts in Return to Monkey Island

## Overview
Return to Monkey island stores its code in form of a custom byte code.
All the functions are compiled into one file named "Weird.dink" in Weird.ggpack1a.

### Weird.dink
This file ist comprised of individual blocks, each of which containing the compiled code of a single function.

Each block starts with 4 bytes of type marker. The following type markers are known:

|marker|description|
|--|--|
|0x3441789C|Used in the .dink file and contains a compiled function each.|

Structure of each block:

|offset|length|description|
|----|----|----|
|0|4|type code (see above)|
|4|4|length of the data in the block (n)|
|8|n|data in the block|

### Compiled script function

Each script function is comprised of a header and multiple blocks. 

|marker|description|
|--|--|
|0x16F94B62|Contains information about the function: Function guid, Name, Script name, number of constants, and unknown ones.|
|0x983f1cfa|Contains constant text data|
|0xFD4BC33A|Contains constants.|
|0x55ED4D1D|The instructions themselves|
|0x62D34042|Contains information about which instructions correspond to which line number in the source file.|

Each function contains all of the above blocks.

#### script function header
The function begins with the magic value 0x7F46a125 and 10 bytes of unknown purpose.

#### Block 0x16F94B62: function information
This block seems to contain general information about the function.

|length|description|
|---|---|
|?| null-terminated string: Function UID|
|?| null-terminated string: Function name|
|?| null-terminated string: Script name|
|1| unknown |
|1| unknown |
|1| number (n) of extra values|
|1| unknown |
|4| number of constants |
|4 \* n|extra values - unknown purpose|
|1| always 0xFF|

#### Block 0x62D34042: Line numbers

Comprised of 12 byte segments.
Used for debugging purposes to show at which line an error occurred.
These seem to jump around a bit, with some line numbers duplicated.
Not all instructions are part of one of these blocks.

|length| description|
|---|---|
|4|line number|
|4|index of the first instruction|
|4|index of the last instruction or index of the first instruction of the next segment - seems inconsistent.|

#### Block 0x983f1cfa: string data
This block contains a lot of text data (all null-terminated strings).

#### Block 0xFD4BC33A: constants
Comprised of 8 byte blocks.

|length| description|
|----|----|
|4|type code|
|4|value|

The script files only contain the type codes 0x102 (int), 0x103 (float) and 0x204 (string).
The game itself uses these codes:

|code|type|
|----|----|
|0x102|int|
|0x103|float|
|0x0A|point|
|0x00|null|
|0x206|table|
|0x205|array|
|0x204|string|
|0x207|closure|
|0x20b|pointer|
|0x08|native|
|0x09|refvalue|

For int and float the value represents the constant's value directly.
Strings are represented by the offset into the string data block.

#### Block 0x55ED4D1D: instructions
Each instruction is 4 bytes.
The first 7 bits denote the op-code.

Instructions can use three different parameters (depending on the opCode):
- instruction >> 7
- (byte)(instruction >> 16)
- instruction >> 23

The following opCodes are defined by the game:

|name|opcode|description|
|---|---|---|
OP_NOP | 0x00| no-operation |
OP_PUSH_CONST | 0x01| push a constant to the stack|
OP_PUSH_NULL | 0x02|  push null to the stack |
OP_PUSH_LOCAL | 0x03| pushes a local to the stack |
OP_PUSH_UPVAR | 0x04| pushes a variable from a calling function to the stack |
OP_PUSH_GLOBAL | 0x05| pushes a global to the stack |
OP_PUSH_FUNCTION | 0x06| |
OP_PUSH_VAR | 0x07|       |
OP_PUSH_GLOBALREF | 0x08| |
OP_PUSH_LOCALREF | 0x09|  |
OP_PUSH_UPVARREF | 0x0A|  |
OP_PUSH_VARREF | 0x0B|    |
OP_PUSH_INDEXREF | 0x0C|  |
OP_DUP_TOP | 0x0D| duplicate the stack's top |
OP_UNOT | 0x0E| unary not|
OP_UMINUS | 0x0F| unary minus|
OP_UONECOMP | 0x10| unary one's compliment (bitwise not)|
OP_MATH | 0x11| performs a mathematical operation on the two values on the stack based on its parameter.|
OP_LAND | 0x12|           |
OP_LOR | 0x13|            |
OP_INDEX | 0x14|          |
OP_ITERATE | 0x15|        |
OP_ITERATEKV | 0x16|      |
OP_CALL | 0x17|  call function - the function name is the parameter on top of the stack|
OP_FCALL | 0x18| call function - the function name is the parameter on top of the stack|
OP_CALLINDEXED | 0x19|    |
OP_CALL_NATIVE | 0x1A|    |
OP_FCALL_NATIVE | 0x1B|   |
OP_POP | 0x1C| pop the stack's top value.|
OP_STORE_LOCAL | 0x1D|    |
OP_STORE_UPVAR | 0x1E|    |
OP_STORE_ROOT | 0x1F|     |
OP_STORE_VAR | 0x20|      |
OP_STORE_INDEXED | 0x21|  |
OP_SET_LOCAL | 0x22|      |
OP_NULL_LOCAL | 0x23|     |
OP_MATH_REF | 0x24|       |
OP_INC_REF | 0x25|        |
OP_DEC_REF | 0x26|        |
OP_ADD_LOCAL | 0x27|      |
OP_JUMP | 0x28| jump always|
OP_JUMP_TRUE | 0x29|  jump if stack.pop() == true|
OP_JUMP_FALSE | 0x2A| jump if stack.pop() == false|
OP_JUMP_TOPTRUE | 0x2B|  jump if stack.top() == true |
OP_JUMP_TOPFALSE | 0x2C| jump if stack.top() == false |
OP_TERNARY | 0x2D|        |
OP_NEW_TABLE | 0x2E|      |
OP_NEW_ARRAY | 0x2F|      |
OP_NEW_SLOT | 0x30|       |
OP_NEW_THIS_SLOT | 0x31|  |
OP_DELETE_SLOT | 0x32|    |
OP_RETURN | 0x33| return from a function|
OP_CLONE | 0x34|          |
OP_BREAKPOINT | 0x35| probably used for debugging. |
OP_REMOVED | 0x36| treated like no-op|

## Adding files to the .ggpack??

It is now possible to drag files into the explorer window when a pack file is opened.
If a file with this name already exists in the pack, it will be overwritten (be careful!).
The pack will immediately be saved and the previous version is copied to \*.backup? (counting up if the backup already exists.)

## Patching script files

I developed a simple JSON-based file format to patch RtMI's compiled scripts (without distributing parts of the original game's code of course).
Here's an example:

```
{
	"title": "Test 01",
	"author": "JanFrederick",
	"description": "Demo-patchfile.\nLogs a new message to the game's log.txt at the start of Boot.dinky::main().",
	"function_patches": [
		{
			"script": "Boot.dinky",
			"function": "main",
			"patches": [
				{ 
					"type": "add_local",
					"value": "Hello world! This game's scripts have been modified!",
					"index": 150
				},	
				{
					"type": "insert_instructions",
					"index": 0,
					"value": "PUSH_CONST 15 \nPUSH_CONST 150\n PUSH_VAR 17\n CALL 2 // append_log(\"log\", \"This game's scripts have been modified!\""
				}
			]
		}
	]
}
```

This file can be saved with the extension .dinkypatch.
When dragged into the explorer, the file will not be added to the pack file but instead be used to modify the Weird.dink file.

The format provides an opportunity to give a title and description to the patch / mod. The author's name can also be included.

Each item in "function_patches" modifies a script function.
The functio_patch item contains the script file and function to modify. Note: these are case-sensitive!
Each item in patches is sequentially applied to the function.

The "patch"-item is categorized into two categories: patches for instructions and patches for constants:

### Patching constants.

The following types are available:

- add_local: Adds a constant to the function.
- set_local: Sets the value of a constant.

if the "value" is a string, the constant's type will be 0x204.
if the "value" is a number and "valuetype" is not "int", the constant's type will be 0x103.
(otherwise: 0x102).

When using set_local you have to provide the index of the local to modify.
When using add_local, the index is used for sanity-checking. The new constant is added at the end of the list, and if index is provided and the actual index differs, the patching fails.

It is possible to specify "old_value" and "oldvaluetype". This aids in further sanity-checking that the game's script file is as expected. If the old value does not match, the patching fails.

### Patchin instructions

The following types are available:

- replace_instruction: replaces an instruction
- insert_instructions: inserts one or multiple instructions at the specified index.

"index" must be specified. 
The "value" contains the opcode(s) to insert / set.
(Setting old_value is supported for replace_instruction). insert_instructions accepts multiple opcodes separated by newlines (\n in JSON).

Each line can end in a comment, the following symbols are valid comment markers: "#". ";", "//".

It is possible to specify the instruction as it's raw byte-code (hexadecimal).
Otherwise, use the op-code's name without the "OP_"-prefix. 

The following opcodes are supported:

- NOP:
- REMOVED:
- RETURN:
- PUSH_NULL:
- BREAKPOINT:
- POP:
- DUP_TOP:
- UNOT:
- UMINUS:
- UONECOMP: no parameter
- JUMP:
- JUMP_FALSE:
- JUMP_TOPFALSE:
- JUMP_TOPTRUE:
- JUMP_TRUE: signed integer - how many instructions to skip (forwards or backwards)
- PUSH_CONST:
- PUSH_LOCAL:
- PUSH_GLOBAL:
- PUSH_FUNCTION:
- PUSH_VAR:
- PUSH_GLOBALREF:
- PUSH_LOCALREF:
- PUSH_VARREF:
- NULL_LOCAL: parameter refers to a constant slot.
- CALL:
- FCALL: parameter describes how many arguments the function takes.
- MATH: parameter describes the math operator's function (hexadecimal!) 

### How the example works

The example patch file adds the following instructions to the start of Boot.dinky::main()

```
	PUSH_CONST "log"
	PUSH_CONST "Hello world! This game's scripts have been modified!"
	PUSH_VAR append_log
	Call 2
```

or in other words:

```
	append_log("log", "Hello world! This game's scripts have been modified!");
```

which results in the following entry in the log.txt (%appdata%\\Terrible Toybox\\Return to Monkey Island\\Log.txt) each time the game boots:

```
[10/08/22 19:22:05] GGGraphics: Selecting DirectX 12 renderer 
[10/08/22 19:22:05] Version 0.1 build 524692 (Windows)
[2022-10-08 19:22] Hello, World! This game's scripts have been modified!
[2022-10-08 19:22] Loading trivia: 100/0
```