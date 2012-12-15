#!/bin/bash
PASS="$(echo -ne "\033[0;32m[PASS]\033[0m")"
FAIL="$(echo -ne "\033[0;31m[FAIL]\033[0m")"

for b in Debug Release; do
	REMD="$(dirname "$0")/../src/remd/bin/$b/remd.exe";
	if [ -f "$REMD" ]; then
		break;
	fi
done

if [ ! -f "$REMD" ]; then
	echo "Unable to locate remd.exe - have you built the solution?";
	exit 1;
fi

echo "# remd.exe is \"$REMD\"";

TESTS_FOLDER="$(dirname "$0")/structural";

find "$TESTS_FOLDER" -type f | grep -E '\.emd$' | while read test; do
	echo -n "[$test] ";

	expected="$test.expected";
	if [ ! -f "$expected" ]; then
		echo $FAIL;
		echo -e "\tNot found: \"$expected\".";
		continue;
	fi

	renderer="$(head -n 1 "$expected" | grep -E '^#!' | sed -r 's/^#!\s+//g')"
	if [ ! "$renderer" ]; then
		echo $FAIL;
		echo -e "\tUnable to determine renderer.";
		continue;
	fi

	differences="$(
		"$REMD" "$test" - \
		| diff --ignore-matching-lines='^\s*#' --ignore-all-space \
			--old-line-format="E%3dn|%L" --new-line-format="A%3dn|%L" --unchanged-line-format=" %3dn|%L" \
			"$expected" - \
		| grep -2 -E '^[AE]' \
		| sed -r 's/^--/ ...|/' \
		| sed -r 's/^E.*$/\x1b[0;32m&\x1b[0m/' | sed -r 's/^A.*$/\x1b[0;31m&\x1b[0m/'
	)";

	if [ "$differences" ]; then
		echo $FAIL;
		echo "$differences";
	else
		echo $PASS;
	fi
done
