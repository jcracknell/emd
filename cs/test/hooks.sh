#!/bin/bash

SELF="$1"

for build in Debug Release; do
  REMD="$(dirname "$SELF")/../src/remd/bin/$build/remd.exe";
  if [ -f "$REMD" ]; then break; fi
done

if [ ! -f "$REMD" ]; then
  echo "cs: Unable to locate remd.exe - have you built the solution?";
  exit 1;
fi

echo "# cs: REMD is \"$REMD\"";

function RENDER() { renderer="$1"; test="$2";
  "$REMD" "$test" -  
}
