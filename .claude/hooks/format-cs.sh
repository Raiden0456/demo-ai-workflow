#!/usr/bin/env bash
set -euo pipefail

file=$(jq -r '.tool_input.file_path // empty')
[[ "$file" == *.cs ]] || exit 0

cd "$CLAUDE_PROJECT_DIR"
dotnet format whitespace DemoAiWorkflow.slnx --include "$file" >&2 || {
  echo "dotnet format failed for $file" >&2
  exit 2
}
