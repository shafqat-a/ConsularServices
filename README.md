# ConsularServices
Consular Services API project for managing consular operations.

## Auto-Approval System

This repository includes an automated PR approval system using GitHub Actions. The system automatically approves pull requests that meet certain safety criteria.

### How It Works

The auto-approval workflow (`.github/workflows/auto-approve.yml`) runs on every pull request and:

1. **Checks the PR author** - Automatically approves PRs from trusted bots like:
   - `dependabot[bot]` (dependency updates)
   - `github-actions[bot]` (automated commits)

2. **Checks for auto-approve labels** - PRs with these labels are automatically approved:
   - `auto-approve`
   - `dependencies` 
   - `safe-changes`

3. **Applies safety checks** - Ensures PRs meet security criteria:
   - No files larger than 1MB
   - Limited number of files changed
   - Restricted paths are not modified

### Configuration

The auto-approval behavior is controlled by `.github/auto-approve.yml`. Key settings include:

- **trusted_actors**: List of GitHub users/bots that can be auto-approved
- **auto_approve_labels**: Labels that trigger auto-approval
- **max_file_size**: Maximum file size limit (default: 1MB)
- **safe_paths**: File paths that are considered safe for auto-approval
- **restricted_paths**: Paths that should never be auto-approved
- **safety_checks**: Additional limits on files changed, lines changed, etc.

### Security Features

- **File size limits**: Prevents approval of PRs with unusually large files
- **Path restrictions**: Critical paths (security configs, production settings) require manual review
- **Change limits**: PRs with too many changes require manual review
- **Permission controls**: Uses minimal required GitHub token permissions

### Manual Override

To manually trigger auto-approval for a PR:
1. Add the `auto-approve` label to the PR
2. The workflow will run and approve if all safety checks pass

### Disabling Auto-Approval

To disable auto-approval:
1. Set `enabled: false` in `.github/auto-approve.yml`, or
2. Remove the workflow file entirely

### Monitoring

The workflow provides clear feedback:
- ✅ Comments when a PR is auto-approved with reasons
- ❌ Comments when auto-approval is skipped with explanations
- 📝 Detailed logs in the GitHub Actions tab
