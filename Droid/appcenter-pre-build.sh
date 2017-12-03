#!/bin/bash

find .. -name Keys.cs -exec sed -i -e 's/emotionkey/$USER-DEFINED_EMOTION_KEY/g' {} \;
