#!/bin/bash
# This is a quick and dirty script to extract a C# char array from
# http://www.unicode.org/Public/UNIDATA/UnicodeData.txt
zeros="00000000";
cat "$1" | cut -d \; -f 3 | sort | uniq | while read category; do
	echo -n "private static readonly CodePoint[] U$category = new CodePoint[] {";
	cat "$1" | grep -E '^([^;]*;){2}'"$category"';' | cut -d \; -f 1 | while read codePoint; do
		# C# does not support codepoints above 0x10FFFF
		if [ $((0x$codePoint+0)) -gt $((0x10FFFF+0)) ]; then
			echo "DISCARDED: $codePoint"; continue;
		fi

		if [ $((0x$codePoint+0)) -lt $((0x10000+0)) ]; then
			# Regular codepoint
			if [ ${#codePoint} -lt 4 ]; then
				# Zero padding
				codePoint="${zeros:0:$((4-${#codePoint}))}$codePoint";
			fi
			echo -n "C('\u$codePoint'),"
		else
			# Surrogate pair
			codePoint=$((0x$codePoint-0x10000));
			hiTen=$(printf "%X" $((($codePoint>>10)+0xD800)));
			loTen=$(printf "%X" $((($codePoint&0x3FF)+0xDC00)));
			echo -n "C('\u$hiTen','\u$loTen'),";
		fi
	done
	echo "};"
done
