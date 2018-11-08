#!/bin/bash

find .. -name Keys.cs -exec sed -i -e 's/emotionkey/'"$EMOTION_KEY"'/g' {} \;
find .. -name Keys.cs -exec sed -i -e 's/ioskey/'"$IOS_APPCENTER_KEY"'/g' {} \;
find .. -name Info.plist -exec sed -i -e 's/APPSECRET/'"$IOS_APPCENTER_KEY"'/g' {} \;
