# Auto-Approval Examples

This document provides examples of how the auto-approval system works in practice.

## Scenario 1: Dependabot PR (Auto-Approved)
When Dependabot creates a PR to update dependencies:
- **Actor**: `dependabot[bot]`
- **Result**: ✅ Auto-approved automatically
- **Comment**: "🤖 Auto-approved this PR! ✅ Reason: All checks passed"

## Scenario 2: PR with auto-approve label (Auto-Approved)
When a maintainer adds the `auto-approve` label:
- **Label**: `auto-approve`
- **Result**: ✅ Auto-approved if safety checks pass
- **Safety checks**: File size < 1MB, no restricted paths modified

## Scenario 3: Large file PR (Not Auto-Approved)
PR that includes files larger than 1MB:
- **Issue**: Files exceed size limit
- **Result**: ❌ Auto-approval skipped
- **Comment**: "🤖 Auto-approval skipped for this PR. ❌ Reason: Large files detected"

## Scenario 4: Security file modification (Not Auto-Approved)
PR that modifies restricted paths:
- **Modified**: `src/security/auth.js`
- **Result**: ❌ Auto-approval skipped (requires manual review)
- **Reason**: Restricted path modification detected

## Testing the Workflow

To test the auto-approval workflow:

1. **Create a test PR** with documentation changes
2. **Add the `auto-approve` label**
3. **Verify** the workflow runs and approves the PR
4. **Check** the PR comment for confirmation

## Monitoring

Monitor auto-approval activity in:
- GitHub Actions tab (workflow runs)
- PR comments (approval notifications)
- Repository insights (approval statistics)