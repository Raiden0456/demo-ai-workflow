---
name: draft-prd
description: Drafts a PRD in Notion from call notes, a short idea or a Notion page, using the team PRD template. Use when a PM asks to write, draft or start a PRD, spec or requirements doc.
argument-hint: "[notes, idea or Notion page]"
---

Draft a PRD from: $ARGUMENTS

1. Get the source. If it names a Notion page, find it with search and read it. If it's text, use it as is. If there is nothing to work from, ask for notes first.
2. Read the "PRD template" page in Notion and follow its sections exactly, in the same order.
3. Write for engineers and PMs:
   - Requirements are numbered (R1, R2, ...) and each one is testable.
   - Every requirement has at least one Given / when / then acceptance criterion.
   - Permissions and tenant rules are explicit.
   - Anything the source doesn't answer goes to Open questions with an owner. Never invent numbers, dates or decisions.
4. Show the draft in the chat with a three-line summary on top: goal, biggest risk, number of open questions.
5. Ask: "Create this as a Draft page in Notion?" Create it only after a clear yes.
6. Reply with the page link and remind the PM to review it and set the status to "Ready for tickets" when done.

Never change existing pages in this skill.
