#!/bin/bash

dotnet publish -o out -r linux-x64
./out/llvm