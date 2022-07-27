# Flows tool - extralog2puml

We need some tools to digest Lio's accidental complexity. Let it be one of those which we create later ;)

## Development contribution info
You need:
* net-5.0
* PlantUML `plantuml.com`
* just clone git this: `https://wrp-gitintl.vgnet.volgrp.com/summary/TDCC%2Fgit-workshop.git`

## Usage

### Basic flows.exe usage

```
flows parse --help
parse:
  Parse extralog files looking for EventSourcing flows. Will find only first in log!

Usage:
  Flows parse [options]

Options:
  -f, --source-file <source-file>    File containing Participations Dependencies and Participations Event flow logs
  -?, -h, --help                     Show help and usage information

```
Example:
`flows parse -f LIONSG1ADAPTERSERVICE_VLOD_20201020_153010.log`

* it will generate .puml file `LIONSG1ADAPTERSERVICE_VLOD_20201020_153010.log.puml`
* open PlantUML on the same folder (actively monitors)
* PlantUML will generate `LIONSG1ADAPTERSERVICE_VLOD_20201020_153010.log.png`
* Tested only on with working directory


