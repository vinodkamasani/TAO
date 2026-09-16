# Campaign Workflow State Analysis

## Overview
Based on the codebase analysis, we can definitely track where a campaign is in its workflow. The campaign goes through several distinct stages:

---

## Campaign Workflow Stages

### Stage 1: Job Profile Creation & Approval
**Status**: `JobProfileStatus` (Generated, Approved)
- **Generated**: Job Profile has been AI-generated but not yet approved
- **Approved**: Job Profile has been approved by hiring manager
- **Database Relation**: `JobProfile.CampaignId` → References the Campaign

### Stage 2: Hiring Strategy Creation & Approval
**Status**: `HiringStrategyStatus` (Generated, Approved)
- **Generated**: Hiring Strategy has been AI-generated but not yet approved
- **Approved**: Hiring Strategy has been approved for use
- **Dependency**: Requires approved Job Profile
- **Database Relation**: `HiringStrategy.CampaignId` → References the Campaign

### Stage 3: Resume Import
**Status**: `ResumeImportStatus` (Queued, Processing, Completed, Failed)
- **Queued**: Resume import batch has been created but not started
- **Processing**: Resumes are currently being parsed and processed
- **Completed**: Resume import has finished successfully
- **Failed**: Resume import encountered errors
- **Database Relation**: `ResumeImport.CampaignId` → References the Campaign

### Stage 4: Assessment Strategy Creation & Approval
**Status**: `AssessmentStrategyStatus` (Generated, Approved)
- **Generated**: Assessment Strategy has been AI-generated but not yet approved
- **Approved**: Assessment Strategy has been approved for use
- **Dependency**: Requires approved Job Profile and Hiring Strategy
- **Database Relation**: `AssessmentStrategy.CampaignId` → References the Campaign

---

## Campaign State Enumeration

Based on the entities and their statuses, here are the possible campaign states:

```
┌─────────────────────────────────────────────────────────┐
│         CAMPAIGN WORKFLOW PROGRESSION                   │
├─────────────────────────────────────────────────────────┤
│ 1. NEW CAMPAIGN CREATED                                 │
│    └─ No Job Profile, Hiring Strategy, or Assessment   │
│                                                         │
│ 2. JOB PROFILE GENERATION IN PROGRESS                   │
│    └─ Job Profile exists but Status = Generated        │
│                                                         │
│ 3. JOB PROFILE PENDING APPROVAL                         │
│    └─ Job Profile exists but Status = Generated        │
│       (Waiting for hiring manager approval)            │
│                                                         │
│ 4. JOB PROFILE APPROVED                                 │
│    └─ Job Profile.Status = Approved                    │
│                                                         │
│ 5. HIRING STRATEGY GENERATION IN PROGRESS               │
│    └─ Hiring Strategy exists but Status = Generated    │
│                                                         │
│ 6. HIRING STRATEGY PENDING APPROVAL                     │
│    └─ Hiring Strategy exists but Status = Generated    │
│       (Waiting for approval)                           │
│                                                         │
│ 7. HIRING STRATEGY APPROVED                             │
│    └─ HiringStrategy.Status = Approved                 │
│                                                         │
│ 8. ASSESSMENT STRATEGY GENERATION IN PROGRESS           │
│    └─ Assessment Strategy exists but Status = Generated│
│                                                         │
│ 9. ASSESSMENT STRATEGY PENDING APPROVAL                 │
│    └─ Assessment Strategy exists but Status = Generated│
│       (Waiting for approval)                           │
│                                                         │
│ 10. ASSESSMENT STRATEGY APPROVED                        │
│     └─ AssessmentStrategy.Status = Approved            │
│                                                         │
│ 11. RESUMES IMPORTED                                    │
│     └─ ResumeImport exists with Status = Completed     │
│        (Candidate applications created)                │
│                                                         │
│ 12. READY FOR ASSESSMENT                                │
│     └─ All above steps completed                       │
│        (Assessments can be started)                    │
└─────────────────────────────────────────────────────────┘
```

---

## Database Queries to Determine Campaign State

### Query Structure
```csharp
var campaignState = campaign
	.With(JobProfile filtered by CampaignId)
	.With(HiringStrategy filtered by CampaignId)
	.With(AssessmentStrategy filtered by CampaignId)
	.With(ResumeImport filtered by CampaignId)
	- Check each entity's Status property
	- Determine what stage campaign is in
```

### Key Data Available
1. **JobProfile**: 
   - Exists? → Check if null
   - Status? → Generated (1) or Approved (2)

2. **HiringStrategy**:
   - Exists? → Check if null
   - Status? → Generated (1) or Approved (2)

3. **AssessmentStrategy**:
   - Exists? → Check if null
   - Status? → Generated (1) or Approved (2)

4. **ResumeImport**:
   - Exists? → Check if null
   - Status? → Queued (1), Processing (2), Completed (3), Failed (4)
   - Progress? → TotalFiles, SuccessfulFiles, FailedFiles

---

## Recommended DTO for Campaign Workflow State

```csharp
public sealed record CampaignWorkflowStateResponse(
	Guid CampaignId,
	string CurrentStage,           // enum-like string: "New", "JobProfilePending", "HiringStrategyPending", etc.

	// Job Profile Stage
	bool HasJobProfile,
	string? JobProfileStatus,               // "Generated", "Approved", null
	DateTime? JobProfileCreatedOn,
	DateTime? JobProfileApprovedOn,

	// Hiring Strategy Stage
	bool HasHiringStrategy,
	string? HiringStrategyStatus,           // "Generated", "Approved", null
	DateTime? HiringStrategyCreatedOn,
	DateTime? HiringStrategyApprovedOn,

	// Assessment Strategy Stage
	bool HasAssessmentStrategy,
	string? AssessmentStrategyStatus,       // "Generated", "Approved", null
	DateTime? AssessmentStrategyCreatedOn,
	DateTime? AssessmentStrategyApprovedOn,

	// Resume Import Stage
	bool HasResumeImport,
	string? ResumeImportStatus,             // "Queued", "Processing", "Completed", "Failed"
	int TotalResumes,
	int SuccessfulResumes,
	int FailedResumes,
	DateTime? ResumeImportCompletedOn,

	// Overall Progress
	int CompletionPercentage,               // 0-100
	int TotalStepsCompleted,                // 0-4
	int TotalSteps);                        // Always 4
```

---

## Enum for Campaign Workflow Stages

```csharp
public enum CampaignWorkflowStage
{
	New = 1,
	JobProfileGenerating = 2,
	JobProfilePendingApproval = 3,
	JobProfileApproved = 4,
	HiringStrategyGenerating = 5,
	HiringStrategyPendingApproval = 6,
	HiringStrategyApproved = 7,
	AssessmentStrategyGenerating = 8,
	AssessmentStrategyPendingApproval = 9,
	AssessmentStrategyApproved = 10,
	ResumesImporting = 11,
	ReadyForAssessment = 12
}
```

---

## Implementation in Code

For the GET Campaign endpoint, you can:

1. **Option A**: Create a separate endpoint: `GET /api/campaigns/{campaignId}/workflow-state`
   - Returns detailed workflow state information
   - Clean separation of concerns
   - Useful for UI to show progress

2. **Option B**: Extend current CampaignResponse to include:
   - `CurrentStage` (enum string)
   - `CompletionPercentage`
   - Summary status info

3. **Option C**: Create a new GET endpoint: `GET /api/campaigns/{campaignId}/progress`
   - Returns progress/workflow data
   - More RESTful approach

---

## Benefits

✅ **UI can show workflow progress bars and status indicators**
✅ **Know exactly what step user is on**
✅ **Prevent invalid operations (e.g., can't create hiring strategy without approved job profile)**
✅ **Guide users through the workflow**
✅ **Display resume import progress (3/10 files processed)**
✅ **Show completion percentage for the campaign setup**

---

## Summary

**Yes, we can absolutely track campaign state!** Each stage has corresponding:
- **Entity** (JobProfile, HiringStrategy, AssessmentStrategy, ResumeImport)
- **Status** (Generated, Approved, Completed, Failed, etc.)
- **Timestamp** (CreatedOn, ApprovedOn, CompletedOn)

This enables building a comprehensive workflow state tracker for the UI.
