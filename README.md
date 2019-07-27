# .NET-Standard-Atomic-File-Operations
This library provides some .NET Standard atomic file operations.

When dealing with writing data to a disk several problems exist.
The worst problems are caused by random power outages or random crashes, 
which may corrupt a file completely. Thus, it would be preferable to have
a procedure where the state of the system is always valid. In other words,
a file operation must either be completely finished or no changes should be
made at all. This is called atomicity, but is not completely achievable with
a standard file system. Thus, we settle for a procedure where the data can
always be recalled to a valid state, which mimicks atomicity.

Note: There are several database softwares, which are ACID compliant and thus
fulfill the atomicity criteria. However, sometimes these can be an overkill or not 
even implementable in all scenarios. 



## Usage
Include the code in your project and call the methods from the static class AtomicFileOperation.
The methods mimick those of System.IO.File.

Currently supported write operations:
+ WriteAllBytes
+ WriteAllText
+ WriteAllLines

Currently supported read operations:
+ ReadAllBytes
+ ReadAllText
+ ReadAllLines

### Examples
```cs
AtomicFileOperation.WriteAllText("C:/temp/test.txt", "Hello World!");
var contents = AtomicFileOperation.ReadAllText("C:/temp/test.txt");
```

## Definitions
F = File that the operation is performed on, aka target file, exists.  
S = State file exists.  
T = Temp file exists.  
(V) = Denotes the valid file.  

## Write Operation Procedure
All specified operations in this library can fail during or after the execution
call, but a valid file state should always be recallable. These steps do not explicitly
note that the state of the file system is always cleaned before a write process.

### If Target File Exists At Start
**Procedure steps**

| Step | Description                                      |
|------|--------------------------------------------------|
| E0   | File system original state.                      |
| E1   | Write and flush new data into the temp file.     |
| E2   | Delete target file.                              |
| E3   | Rename temp file to target file.                 |

**Existing files after completed steps**

| Step | Target file | Temporary file |
|------|-------------|----------------|
| E1   | F (V)       |                |
| E2   | F (V)       | T              |
| E3   |             | T (V)          |
| E4   | F (V)       |                |

### If Target File Does Not Exists At Start
**Procedure steps**

| Step | Description                                                                |
|------|----------------------------------------------------------------------------|
| N0   | File system original state.                                                |
| N1   | Create a state file that denotes that no target file existed at start.     |
| N2   | Write and flush new data into the temp file.                               |
| N3   | Delete state file.                                                         |
| N4   | Rename temp file to target file.                                           |

| Step | Target file | State file | Temporary file |
|------|-------------|------------|----------------|
| N0   | (V)         |            |                |
| N1   | (V)         | S          |                |
| N2   | (V)         | S          | T              |
| N3   |             |            | T (V)          |
| N4   | F (V)       |            |                |

## Read Operation Procedure
The read operation does not change the state of the file system in any regard.
This means that, according to the write procedures above, the actual file that
is read is not necessarily the target file, but could be the temporary file 
instead.

**Procedure steps:**  
If no files exist => Throw error  
Else if only F exists => Read F  
Else if F and T exist => Read F  
Else if only T exists => Read T  
Else if only S exists => Throw error  
Else if S and T exist => Throw error  

Note: An error is thrown if a valid file does not exist at all. This should
only happen on initial write. For example, the file "C:/test.txt" does not exist,
then AtomicFileOperation.WriteAllText("C:/test.txt", "Hello") is called, but a
power outage kills the write process between N1 and N2 and thus we only have the
file S. In other words, no valid file has ever been saved and an error is thrown.

## Clean Operation Procedure
In contrast to the read operation, the clean operation rolls back the file system
so that only the target file or no files at all exist. In other words, the file 
system is cleaned to a valid state without temp or state files.

**Procedure steps:**  
If no files exist => State OK, end clean  
Else if only F exist => State OK, end clean  
Else if F and T exist => Delete T, restart clean  
Else if only T exists => Rename T to F, restart clean  
Else if only S exists => Delete S, restart clean  
Else if S and T exist => Delete T, restart clean  
