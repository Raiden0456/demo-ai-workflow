---
name: prd-to-tickets
description: Turns a PRD in Notion into Jira tickets after the PM approves the breakdown, then links the tickets back in the PRD. Use when asked to create tickets, stories or a backlog from a PRD, spec or requirements page.
argument-hint: "[PRD title or Notion link]"
---

Create Jira tickets from the PRD: $ARGUMENTS

1. Find the PRD in Notion and read it fully. If its status is not "Ready for tickets", say so and ask whether to continue.
2. Check for duplicates: search Jira for the label `prd:<page id>`. If tickets already exist, list them and only propose what is missing.
3. Propose the breakdown as a table: #, type, summary, points (1, 2, 3, 5 or 8), acceptance criteria, PRD requirements covered.
   - One ticket per slice that can ship and be tested on its own. Usually 3 to 7 tickets.
   - Acceptance criteria come from the PRD, word for word where possible. Each ticket includes its tests.
   - Every requirement (R1, R2, ...) is covered by at least one ticket. Say so explicitly, or show what is not covered.
   - Open questions are not tickets. List them under the table and mark tickets that depend on them.
4. Stop and ask: "Create these N tickets in Jira and link them in the PRD? (yes / change something / no)". Apply changes and show the table again. Never create anything without a clear yes.
5. Create each ticket with label `prd:<page id>` and a description with context, acceptance criteria and the PRD link.
6. Append the list of created tickets (key and summary per line) to the end of the PRD, which is its "Jira tickets" section. Do not add another heading.
7. Reply with the created keys, total points and the open questions that still block work.

Works with the demo `jira` and `notion` MCP servers and with the Atlassian and Notion connectors in the Claude app.
