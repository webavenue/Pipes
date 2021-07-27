#!/bin/bash

set -e
dir=$(cd -P -- "$(dirname -- "$0")" && pwd -P)
cd $dir

# Validate maven
mvn -v 2>/dev/null || (>&2 echo -e "Maven not found!\nSee installation instructions here: https://maven.apache.org/install.html" && exit 1)

mvn clean package
java -javaagent:./jetty-alpn-agent-2.0.9.jar -cp ./target/demoserver-*.jar com.universal_tools.demoserver.HttpServer