#!/bin/bash
for b in Debug Release; do
	REMD="$(dirname "$0")/../src/remd/bin/$b/remd.exe";
	if [ -f "$REMD" ]; then
		break;
	fi
done

if [ ! -f "$REMD" ]; then
	echo "Unable to locate remd.exe - have you built the solution?";
fi

echo "# remd.exe is \"$REMD\"";

TESTS_FOLDER="$(dirname "$0")/structural";

find "$TESTS_FOLDER" -type f | grep -E '\.emd$' | while read test; do
	echo -n "[$test] ";

	expected="$(dirname "$test")/$(basename "$test" .emd).expected";
	if [ ! -f "$expected" ]; then
		echo "FAIL";
		echo -e "\tNot found: \"$expected\".";
		continue;
	fi

	renderer="$(head -n 1 "$expected" | grep -E '^#!' | sed -r 's/^#!\s+//g')"
	if [ ! "$renderer" ]; then
		echo "FAIL";
		echo -e "\tUnable to determine renderer.";
		continue;
	fi

	differences=$(
		if [ 1 ]; then
			echo "#! $renderer";
			cat "$test" | "$REMD" -r "$renderer" - -;
		fi | diff -w -y --suppress-common-lines - "$expected"
	);

	if [ "$differences" ]; then
		echo "[FAIL]";
		echo "$differences";
	else
		echo "[PASS]";
	fi
done
