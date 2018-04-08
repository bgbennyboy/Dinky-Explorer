# ggdump

GGDump is a tool for listing and extracting files from data archive files of the game [Thimbleweed Park](https://thimbleweedpark.com/), which you should totally buy right now. ([Steam](http://store.steampowered.com/app/569860/Thimbleweed_Park/), [GOG](https://www.gog.com/game/thimbleweed_park))

## Install

You can install ggdump directly from GitHub:

    pip install https://github.com/mstr-/twp-ggdump/archive/master.zip

## Usage

    usage: python -m ggdump [-h] [-w] [-v] ggpack_file search_pattern
    
    positional arguments:
      ggpack_file     ggpack archive file path
      search_pattern  file search pattern, wildcard patterns should be "quoted".
    
    optional arguments:
      -h, --help      show this help message and exit
      -w              write the files that match the pattern (otherwise only list
                      files)
      -v, --verbose   output some debug information
    
    Example: python -m ggdump ThimbleweedPark.ggpack1 "*Sheet.png"
    
## Why does this tool exist

* The author wanted to solve one more puzzle after winning the game.
* To help people get started with modding the game and learning about the file formats.
* At some point we want ScummVM support.
