#!/usr/bin/env bash
set -euo pipefail

command=$(jq -r '.tool_input.command // empty')

block() {
  echo "Blocked by .claude/hooks/guard-bash.sh: $1" >&2
  exit 2
}

current_branch() {
  git -C "${CLAUDE_PROJECT_DIR:-.}" rev-parse --abbrev-ref HEAD 2>/dev/null || true
}

env_file='(^|[[:space:]/"'\''=<>])\.env([.[:space:]"'\'';|&<>]|$)'
[[ "${command//.env.example/}" =~ $env_file ]] && block "secrets in .env files are off limits"
[[ "$command" =~ appsettings\.[A-Za-z]+\.local\.json ]] && block "local appsettings may contain secrets"

push='(^|[^[:alnum:]_-])git([[:space:]]+[^[:space:]]+)*[[:space:]]+push([[:space:]]|$)'
protected_ref='([[:space:]:+/])(master|main)([[:space:]"'\'';|&)]|$)'
if [[ "$command" =~ $push ]]; then
  [[ "$command" =~ $protected_ref ]] && block "master is protected, push a feature branch and open a PR"
  [[ "$command" =~ [[:space:]]--(all|mirror)([[:space:]]|$) ]] && block "pushing all branches includes master"
  [[ "$(current_branch)" =~ ^(master|main)$ ]] && block "you are on master, switch to a feature branch before pushing"
  [[ "$command" =~ [[:space:]]-[a-zA-Z]*f([[:space:]]|$) ]] && block "force push is not allowed"
fi

git_exec='--(upload-pack|receive-pack|exec)([=[:space:]]|$)|GIT_(SSH|SSH_COMMAND|EXTERNAL_DIFF|PAGER|EDITOR|ASKPASS)=|-c[[:space:]]*[^[:space:]]*([Cc]ommand|[Cc]md|[Pp]ager|[Ee]ditor|[Aa]skpass|[Ff]smonitor|[Hh]ooks[Pp]ath|[Hh]elper)[^[:space:]]*='
[[ "$command" =~ git && "$command" =~ $git_exec ]] && block "git options that run arbitrary commands are not allowed"
[[ "$command" =~ git[[:space:]]+(log|diff|show)[[:space:]].*--output ]] && block "writing files through git --output is not allowed"

[[ "$command" =~ gh[[:space:]]+pr[[:space:]]+merge.*--admin ]] && block "merging with --admin bypasses branch protection"
[[ "$command" =~ git[[:space:]]+reset[[:space:]]+--hard ]] && block "destructive reset"
[[ "$command" =~ git[[:space:]]+clean[[:space:]]+-[a-z]*f ]] && block "destructive clean"
[[ "$command" =~ --force|--no-verify ]] && block "force and hook bypass flags are not allowed"

exit 0
