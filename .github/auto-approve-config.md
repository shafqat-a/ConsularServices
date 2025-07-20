# Auto-Approval Configuration

## Overview
This repository is configured to automatically approve all pull requests that are:
- Not in draft status
- Properly opened, reopened, synchronized, or marked as ready for review

## How it works
The auto-approval system uses GitHub Actions workflow (`.github/workflows/auto-approve-prs.yml`) that:

1. Triggers on PR events (opened, reopened, synchronize, ready_for_review)
2. Checks if the PR is not a draft
3. Automatically approves the PR using the `hmarr/auto-approve-action`
4. Adds a review message indicating automatic approval

## Permissions Required
The workflow requires:
- `pull-requests: write` - To approve pull requests
- `contents: read` - To read repository contents

## Customization
To modify the auto-approval behavior, edit the workflow file:
- Change trigger events in the `on` section
- Modify conditions in the `if` clause
- Update the review message
- Add additional checks or conditions as needed

## Security Considerations
- This system approves ALL non-draft pull requests automatically
- Consider adding additional conditions for production environments
- Review the workflow regularly to ensure it meets your security requirements