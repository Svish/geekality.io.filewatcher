Introduction
===

The `FileSystemWatcher` class can only watch a single directory and its filter can only match a single file or a file pattern. 
I needed to watch multiple files across directories so I wrote this simple wrapper to manage this for me. It uses one `FileSystemWatcher` per directory and it updates itself when files are renamed or deleted.

License
===

This work is licensed under the Creative Commons Attribution 3.0 Unported License. To view a copy of this license, visit [Creative Commons Attribution 3.0 Unported License](http://creativecommons.org/licenses/by/3.0/).

![Creative Commons License](http://i.creativecommons.org/l/by/3.0/88x31.png)