#!/usr/bin/env bash
set -euo pipefail

reviewer="${1:?usage: set-ai-reviewer.sh claude|copilot|none [owner/repo]}"
repo="${2:-$(gh repo view --json nameWithOwner -q .nameWithOwner)}"
ruleset_name="Copilot code review"

copilot_ruleset_id() {
  gh api "repos/$repo/rulesets" -q ".[] | select(.name == \"$ruleset_name\") | .id"
}

disable_copilot() {
  local id
  id=$(copilot_ruleset_id)
  if [[ -n "$id" ]]; then
    gh api -X DELETE "repos/$repo/rulesets/$id" >/dev/null
  fi
}

enable_copilot() {
  if [[ -n "$(copilot_ruleset_id)" ]]; then
    return
  fi
  gh api -X POST "repos/$repo/rulesets" --input - >/dev/null <<JSON
{
  "name": "$ruleset_name",
  "target": "branch",
  "enforcement": "active",
  "conditions": { "ref_name": { "include": ["~DEFAULT_BRANCH"], "exclude": [] } },
  "rules": [
    {
      "type": "copilot_code_review",
      "parameters": { "review_on_push": true, "review_draft_pull_requests": false }
    }
  ]
}
JSON
}

case "$reviewer" in
  claude) disable_copilot ;;
  copilot) enable_copilot ;;
  none) disable_copilot ;;
  *)
    echo "unknown reviewer: $reviewer" >&2
    exit 1
    ;;
esac

gh variable set AI_REVIEWER --repo "$repo" --body "$reviewer"
echo "AI reviewer for $repo: $reviewer (rules: REVIEW.md, merge still needs a human approval)"
