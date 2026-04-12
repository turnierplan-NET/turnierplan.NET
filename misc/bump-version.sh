#!/bin/bash

#
# Bumps the turnierplan.NET version in all relevant files for the next release.
#
# Usage:
#   ./bump-version.sh <new version>
#

if (( $# != 1 )); then
  echo "Usage: $0 <new version>"
  exit 1
fi

# The 'version.xml' file is considered leading
current_version=$(grep -Po '\d+\.\d+\.\d+' ../src/version.xml)

next_version="$1"
if [[ ! $next_version =~ ^20[2-9][0-9]+\.[1-9][0-9]*\.[0-9]+$ ]]; then
  echo "Error: The specified new version '$1' is not a valid version."
  exit 1
fi

if [[ $current_version == $next_version ]]; then
  echo "Error: The current version is already '$1'."
    exit 1
fi

echo "Bumping version: '$current_version' -> '$next_version'"

update_file () {
  echo "Updating file: $1"
  sed -i -e "s/${current_version//./\\.}/$next_version/g" "../$1"
}

update_file "src/version.xml"
update_file "src/Turnierplan.App/Client/package.json"
update_file "src/Turnierplan.App/Client/src/environments/environment.prod.ts"
update_file "docs/pages/installation/index.md"
