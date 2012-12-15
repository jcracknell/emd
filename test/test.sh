#!/bin/bash
(
	PWD="$(dirname "$0")";

	COLOR_NONE="$(echo -ne "\033[0m")";
	COLOR_GREEN="$(echo -ne "\033[0;32m")";
	COLOR_RED="$(echo -ne "\033[0;31m")";

	PASS="${COLOR_GREEN}[PASS]${COLOR_NONE}";
	FAIL="${COLOR_RED}[FAIL]${COLOR_NONE}";

	impl=$(if [ $1 ]; then echo "$1"; else echo "cs"; fi);
	echo "# Target implementation: $impl";

	implHooks="$PWD/../$impl/test/hooks.sh";
	if [ ! -f "$implHooks" ]; then
		echo "Not found: \"$implHooks\"";
		exit 1;
	fi

	echo "# Loading hooks: \"$implHooks\"";
	# Second argument passes the path of the hooks file to itself
	. "$implHooks" "$implHooks"

	find "$PWD" -type f | grep -E '\.emd$' | while read test; do
		echo -n "[$impl:$test] ";

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
			RENDER "$renderer" "$test" \
			| diff --ignore-matching-lines='^[ 	]*#' --ignore-all-space \
				--old-line-format="E%3dn|%L" --new-line-format="A%3dn|%L" --unchanged-line-format=" %3dn|%L" \
				"$expected" - \
			| grep -2 -E '^[AE]' \
			| sed -r 's/^--/ ...|/' \
			| sed -r 's/^E.*$/'"$COLOR_GREEN"'&'"$COLOR_NONE"'/' | sed -r 's/^A.*$/'"$COLOR_RED"'&'"$COLOR_NONE"'/'
		)";

		if [ "$differences" ]; then
			echo $FAIL;
			echo "$differences";
		else
			echo $PASS;
		fi
	done
) | less
