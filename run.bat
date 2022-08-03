@echo off

pushd build
csc ..\interpreter\main.cs
main.exe
popd


