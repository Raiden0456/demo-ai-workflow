#!/usr/bin/env bash
set -euo pipefail

command=$(jq -r '.tool_input.command // empty')

block() {
  echo "Blocked by .claude/hooks/guard-bash.sh: $1" >&2
  exit 2
}

env_file='(^|[[:space:]/"'\''=<>])\.env([.[:space:]"'\'';|&<>]|$)'
[[ "${command//.env.example/}" =~ $env_file ]] && block "secrets in .env files are off limits"
[[ "$command" =~ appsettings\.[A-Za-z]+\.local\.json ]] && block "local appsettings may contain secrets"
[[ "$command" =~ git[[:space:]]+push ]] && block "pushing is a human decision"
[[ "$command" =~ git[[:space:]]+reset[[:space:]]+--hard ]] && block "destructive reset"
[[ "$command" =~ git[[:space:]]+clean[[:space:]]+-[a-z]*f ]] && block "destructive clean"
[[ "$command" =~ --force|--no-verify ]] && block "force and hook bypass flags are not allowed"

exit 0
