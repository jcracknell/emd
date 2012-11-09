#!/bin/bash
# This is a quick and dirty script to extract a C# char array from
# http://www.unicode.org/Public/UNIDATA/UnicodeData.txt
zeros="00000000";
cat "$1" | sed -r 's/^([^;]*;){2,2}([^;]+);.*$/\2/i' | sort | uniq | while read category; do
	echo -n "private static readonly char[] U$category = new char[] {";
	cat "$1" \
	| grep -i -E '^([^;]*;){2}'"$category"';' \
	| sed -r 's/^([0-9A-Fa-f]+).*$/\1/' \
	| sort \
	| while read code; do
		hexcode="0x$code";
		if [ $(($hexcode+0)) -gt $((0x10000-1)) ]; then continue; fi
		if [ ${#code} -lt 4 ]; then
			code="${zeros:0:$((4-${#code}))}$code";
		elif [ ${#code} -eq 5 ]; then
			code="${zeros:0:$((6-${#code}))}$code";
		fi
		echo -n "'\u$code',";
	done
	echo '};';
done
