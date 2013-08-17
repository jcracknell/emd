#!/bin/bash

function getCategoryValue() {
  case $1 in
    Lu) echo 0;;
    Ll) echo 1;;
    Lt) echo 2;;
    Lm) echo 3;;
    Lo) echo 4;;
    Mn) echo 5;;
    Mc) echo 6;;
    Me) echo 7;;
    Nd) echo 8;;
    Nl) echo 9;;
    No) echo 10;;
    Zs) echo 11;;
    Zl) echo 12;;
    Zp) echo 13;;
    Cc) echo 14;;
    Cf) echo 15;;
    Cs) echo 16;;  
    Co) echo 17;;
    Pc) echo 18;;
    Pd) echo 19;;
    Ps) echo 20;;
    Pe) echo 21;;
    Pi) echo 22;;
    Pf) echo 23;;
    Po) echo 24;;
    Sm) echo 25;;
    Sc) echo 26;;
    Sk) echo 27;;
    So) echo 28;;
    *) echo 29;;
  esac
}

data="$(dirname "$0")/UnicodeData.txt";
output="$(dirname "$0")/UnicodeCategoryInfo.bin";

if [ ! -f "$data" ]; then
  curl -s 'ftp://ftp.unicode.org/Public/UNIDATA/UnicodeData.txt' > "$data";
fi

cat "$data" \
| grep ';' \
| grep -v -E '(First|Last)>' \
| sed -r 's/^([^;]+);[^;]*;([^;]+);.*$/\1 \2/' \
| while read -a fields; do
  codePoint=$((0x${fields[0]}+0));
  categoryName=${fields[1]};
  category=$(getCategoryValue $categoryName);

  # All codepoints expressible in 24 bits
  echo -ne "\x$(printf '%02X' $((($codePoint >> 16) & 0xFF)))";
  echo -ne "\x$(printf '%02X' $((($codePoint >> 8) & 0xFF)))";
  echo -ne "\x$(printf '%02X' $(($codePoint & 0xFF)))";
  echo -ne "\x$(printf '%02X' $category)";
done > "$output";

cat "$data" \
| grep ';' \
| grep -E '(First|Last)>' \
| sed -r 's/^([^;]+);[^;]*;([^;]+);.*$/\1 \2/' \
| while read -a fields; do
  startCodePoint=$((0x${fields[0]}+0));
  read -a fields;
  endCodePoint=$((0x${fields[0]}+0));
  categoryName=${fields[1]};
  category=$(getCategoryValue $categoryName);

  # First bit set indicates range
  echo -ne "\x$(printf '%02X' $((0x80 | (($startCodePoint >> 16) & 0xFF))))";
  echo -ne "\x$(printf '%02X' $((($startCodePoint >> 8) & 0xFF)))";
  echo -ne "\x$(printf '%02X' $(($startCodePoint & 0xFF)))";
  echo -ne "\x$(printf '%02X' $((($endCodePoint >> 16) & 0xFF)))";
  echo -ne "\x$(printf '%02X' $((($endCodePoint >> 8) & 0xFF)))";
  echo -ne "\x$(printf '%02X' $(($endCodePoint & 0xFF)))";
  echo -ne "\x$(printf '%02X' $category)";
done >> "$output";
