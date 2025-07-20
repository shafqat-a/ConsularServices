# ConsularServices
Consular Services API project for managing consular operations.

## Auto-Approval System

This repository is configured with an automated pull request approval system that automatically approves all non-draft pull requests. 

### Features
- ✅ Automatically approves all non-draft PRs
- ✅ Triggers on PR open, reopen, sync, and ready-for-review events
- ✅ Adds descriptive review messages
- ✅ Configurable through GitHub Actions workflow

### Configuration
The auto-approval system is configured in `.github/workflows/auto-approve-prs.yml`. 
For detailed configuration options, see `.github/auto-approve-config.md`.

### How it Works
1. When a PR is opened, reopened, synchronized, or marked ready for review
2. The GitHub Actions workflow automatically triggers
3. If the PR is not a draft, it gets automatically approved
4. A review comment is added indicating automatic approval
