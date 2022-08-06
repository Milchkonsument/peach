#!bin/fish

echo "kusu's hot restart -- the hottest, perhaps"

FLIST="$(ls interpreter | grep '.cs')"

cd interpreter

while true; do
	
	echo ""
	inotifywait -q --event close_write --format '%w ---' $FLIST | xargs echo "--- CHANGED:" && echo "--- HOT RESTARTING ---" && echo "" && mcs -out:peach $FLIST && mono peach test.peach

done
