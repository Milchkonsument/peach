@echo off

pushd build
csc -out:peach.exe ..\interpreter\main.cs
peach.exe
popd


