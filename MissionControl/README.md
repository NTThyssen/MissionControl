# Mission Control
This piece of software is to be used by DanSTAR to test and launch rockets.

## Quick overview
The software is split into several parts
- The UI built using GTK# (GTK2)
- The data model which stored the shared queue between
- The logger thread with the responsibility of logging incoming data to file
- The IO thread which handles communication

### Features missing
- Error log in file and display
- Stack Health (component)
- Port to GTK3 (not officially supported in Mono)
- Acknowledgment for commands (requires update of the protocol in Software TMX: https://docs.google.com/spreadsheets/d/1u9g2ocEuqxOUPhXzBdTgOuVGiXR6YIJVcAL6W5GDAEI/edit#gid=0)
- UI for using ethernet instead of serial port (Extension of interface written, but no UI or preferneces created)
- ComponentMapping as an readable XML-file
- Configurable decimals for each component
- Simple measure component for logging arbitrary data points

### Known bugs
- Some times crashes, but this cannot be fixed.