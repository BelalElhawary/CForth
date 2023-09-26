# CForth
cforth is like forth programming language but in c#

cforth is first compiles your program into fasm assembly then compiles that assembly and link it to current platfrom

# Run
currently forth has two modes one for stack based code simlar to forth 
```
CForth examples/stack.cf -s -o build/out 
```
and one for c like syntax language
```
CForth examples/test.cf -o build/out 
```

# Supported platforms
- Linux (amd64, x86_64)