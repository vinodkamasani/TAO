# Campaign Endpoints Documentation

**API Version:** 1.0  
**Base URL:** `/api/campaigns`  
**Content-Type:** `application/json` (unless otherwise specified)

---

## Table of Contents
1. [Campaign Management](#campaign-management)
2. [Job Profile Management](#job-profile-management)
3. [Hiring Strategy Management](#hiring-strategy-management)
4. [Resume Management](#resume-management)
5. [Assessment Strategy Management](#assessment-strategy-management)
6. [Enums & Status Values](#enums--status-values)
7. [Error Handling](#error-handling)

---

## Campaign Management

### 1. Create Campaign

**Endpoint:** `POST /api/campaigns/`

**Summary:** Creates a new campaign for organizing recruitment activities.

**Description:** Initializes a new campaign with basic recruitment information. This is the first step in setting up a recruitment process.

**Request Body:**
```json
{
  "organizationId": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Senior Software Engineer - 2024",
  "referenceNumber": "REF-2024-SSE-001",
  "recruiterId": "750e8400-e29b-41d4-a716-446655440001",
  "hiringManagerId": "850e8400-e29b-41d4-a716-446655440002",
  "numberOfOpenings": 5
}
```

**Request Fields:**

| Field | Type | Required | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `organizationId` | UUID | Yes | The organization under which the campaign is created | Must be a valid UUID of an existing organization |
| `name` | String | Yes | Campaign name/title | Min length: 1, Max length: 256 |
| `referenceNumber` | String | Yes | Unique reference identifier for the campaign | Min length: 1, Max length: 100 |
| `recruiterId` | UUID | Yes | ID of the recruiter managing the campaign | Must be a valid UUID of an existing recruiter user |
| `hiringManagerId` | UUID | Yes | ID of the hiring manager approving candidates | Must be a valid UUID of an existing hiring manager user |
| `numberOfOpenings` | Integer | Yes | Number of open positions for this campaign | Min value: 1, Max value: 1000 |

**Response:** `201 Created`
```json
{
  "value": "550e8400-e29b-41d4-a716-446655440100",
  "message": "Campaign created successfully"
}
```

**Response Fields:**

| Field | Type | Description |
|-------|------|-------------|
| `value` | UUID | The ID of the newly created campaign |
| `message` | String | Success message |

**Location Header:** `Location: /api/campaigns/{campaignId}`

**Possible Errors:**
- `400 Bad Request` - Invalid input data
- `409 Conflict` - Duplicate reference number for the organization
- `422 Unprocessable Entity` - Validation error (check error details)

---

## Job Profile Management

### 2. Create Job Profile

**Endpoint:** `POST /api/campaigns/{campaignId}/job-profile`

**Summary:** Generates an AI Job Profile for a campaign.

**Description:** Uses AI to generate a comprehensive job profile based on the original job description provided. The job profile includes structured competencies, requirements, and qualifications.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `campaignId` | UUID | Yes | The campaign ID for which to generate the job profile |

**Request Body:**
```json
{
  "originalJobDescription": "We are looking for a Senior Software Engineer with 5+ years of experience in cloud-native applications..."
}
```

**Request Fields:**

| Field | Type | Required | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `originalJobDescription` | String | Yes | The original job description text to be analyzed | Min length: 50, Max length: 10000 |

**Response:** `201 Created`
```json
{
  "value": "660e8400-e29b-41d4-a716-446655440200",
  "message": "Job profile generated successfully"
}
```

**Possible Errors:**
- `400 Bad Request` - Invalid job description
- `404 Not Found` - Campaign not found
- `420 Enhance Your Calm` - AI service rate limit exceeded; retry after some time
- `500 Internal Server Error` - AI generation failed

---

### 3. Get Job Profile

**Endpoint:** `GET /api/jobprofiles/{jobProfileId}`

**Summary:** Gets a Job Profile by Id.

**Description:** Retrieves the generated or approved job profile details including original description, generated content, and structured profile information.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `jobProfileId` | UUID | Yes | The ID of the job profile to retrieve |

**Query Parameters:** None

**Response:** `200 OK`
```json
{
  "value": {
	"id": "660e8400-e29b-41d4-a716-446655440200",
	"campaignId": "550e8400-e29b-41d4-a716-446655440100",
	"originalJobDescription": "We are looking for a Senior Software Engineer...",
	"generatedContent": "AI-generated comprehensive job profile content...",
	"structuredProfile": "{\"competencies\": [...], \"qualifications\": [...]}",
	"status": 1,
	"generatedOn": "2024-01-15T10:30:00Z"
  },
  "message": "Job profile retrieved successfully"
}
```

**Response Fields:**

| Field | Type | Description |
|-------|------|-------------|
| `id` | UUID | Job profile unique identifier |
| `campaignId` | UUID | Associated campaign ID |
| `originalJobDescription` | String | The original job description provided |
| `generatedContent` | String | AI-generated narrative job profile content |
| `structuredProfile` | String (JSON) | Structured JSON representation of skills, competencies, and requirements |
| `status` | Number | Job profile status (1 = Generated, 2 = Approved) |
| `generatedOn` | DateTime (ISO 8601) | Timestamp when the profile was generated |

**Possible Errors:**
- `404 Not Found` - Job profile not found

---

### 4. Approve Job Profile

**Endpoint:** `POST /api/jobprofiles/{jobProfileId}/approve`

**Summary:** Approves a Job Profile.

**Description:** Marks a generated job profile as approved by the hiring manager, making it available for use in the hiring strategy generation process.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `jobProfileId` | UUID | Yes | The ID of the job profile to approve |

**Request Body:**
```json
{
  "approvedByUserId": "950e8400-e29b-41d4-a716-446655440003"
}
```

**Request Fields:**

| Field | Type | Required | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `approvedByUserId` | UUID | Yes | The user ID of the approver | Must be a valid UUID of an existing user |

**Response:** `204 No Content`

No response body is returned.

**Possible Errors:**
- `400 Bad Request` - Invalid request data
- `404 Not Found` - Job profile not found
- `409 Conflict` - Job profile already approved

---

## Hiring Strategy Management

### 5. Create Hiring Strategy

**Endpoint:** `POST /api/campaigns/{campaignId}/hiring-strategy`

**Summary:** Generates an AI Hiring Strategy for a campaign.

**Description:** Generates an AI-powered hiring strategy based on the approved job profile. This strategy outlines the recruitment approach, candidate sourcing methods, and evaluation criteria.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `campaignId` | UUID | Yes | The campaign ID for which to generate the hiring strategy |

**Request Body:** Empty (no body required)

**Query Parameters:** None

**Response:** `201 Created`
```json
{
  "value": "760e8400-e29b-41d4-a716-446655440300",
  "message": "Hiring strategy generated successfully"
}
```

**Response Fields:**

| Field | Type | Description |
|-------|------|-------------|
| `value` | UUID | The ID of the newly generated hiring strategy |
| `message` | String | Success message |

**Preconditions:**
- Campaign must exist
- Job profile must be approved for the campaign

**Possible Errors:**
- `400 Bad Request` - Campaign in invalid state
- `404 Not Found` - Campaign or job profile not found
- `409 Conflict` - Hiring strategy already exists for campaign
- `420 Enhance Your Calm` - AI service rate limit exceeded

---

### 6. Get Hiring Strategy

**Endpoint:** `GET /api/campaigns/{campaignId}/hiring-strategy`

**Summary:** Gets the Hiring Strategy for a campaign.

**Description:** Retrieves the generated or approved hiring strategy details including the strategy content and structured recommendations.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `campaignId` | UUID | Yes | The campaign ID for which to retrieve the hiring strategy |

**Query Parameters:** None

**Response:** `200 OK`
```json
{
  "value": {
	"id": "760e8400-e29b-41d4-a716-446655440300",
	"campaignId": "550e8400-e29b-41d4-a716-446655440100",
	"generatedContent": "AI-generated hiring strategy content...",
	"structuredContent": "{\"sourcing_strategies\": [...], \"evaluation_criteria\": [...]}",
	"status": 1,
	"providerName": "OpenAI",
	"modelName": "gpt-4",
	"promptVersion": 1,
	"createdOnUtc": "2024-01-15T11:00:00Z"
  },
  "message": "Hiring strategy retrieved successfully"
}
```

**Response Fields:**

| Field | Type | Description |
|-------|------|-------------|
| `id` | UUID | Hiring strategy unique identifier |
| `campaignId` | UUID | Associated campaign ID |
| `generatedContent` | String | AI-generated narrative hiring strategy content |
| `structuredContent` | String (JSON) | Structured JSON with sourcing strategies and evaluation criteria |
| `status` | Number | Hiring strategy status (1 = Generated, 2 = Approved) |
| `providerName` | String | The AI provider used (e.g., "OpenAI", "Anthropic") |
| `modelName` | String | The specific model name used for generation |
| `promptVersion` | Number | Version of the prompt template used |
| `createdOnUtc` | DateTime (ISO 8601) | Timestamp when the strategy was created |

**Possible Errors:**
- `404 Not Found` - Hiring strategy not found for the campaign

---

### 7. Approve Hiring Strategy

**Endpoint:** `POST /api/campaigns/{hiringStrategyId}/approve`

**Summary:** Approves a Hiring Strategy.

**Description:** Marks a generated hiring strategy as approved, making it ready for recruiter use in the candidate evaluation process.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `hiringStrategyId` | UUID | Yes | The ID of the hiring strategy to approve |

**Request Body:**
```json
{
  "approvedByUserId": "950e8400-e29b-41d4-a716-446655440003"
}
```

**Request Fields:**

| Field | Type | Required | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `approvedByUserId` | UUID | Yes | The user ID of the approver | Must be a valid UUID of an existing user |

**Response:** `204 No Content`

No response body is returned.

**Possible Errors:**
- `400 Bad Request` - Invalid request data
- `404 Not Found` - Hiring strategy not found
- `409 Conflict` - Hiring strategy already approved

---

## Resume Management

### 8. Import Resumes

**Endpoint:** `POST /api/campaigns/{campaignId}/resume-imports`

**Summary:** Imports resumes into a campaign.

**Description:** Uploads one or more resume files and initiates the resume import and parsing process. Supports PDF and DOCX formats.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `campaignId` | UUID | Yes | The campaign ID to import resumes into |

**Request Headers:**
```
Content-Type: multipart/form-data
```

**Form Parameters:**

| Parameter | Type | Required | Description | Constraints |
|-----------|------|----------|-------------|-------------|
| `Resumes` | File[] | Yes | Array of resume files to upload | Supported formats: PDF, DOCX; Max file size: 10MB per file; Min 1 file, Max 100 files per request |

**Example Request (using curl):**
```bash
curl -X POST http://api.example.com/api/campaigns/550e8400-e29b-41d4-a716-446655440100/resume-imports \
  -F "Resumes=@resume1.pdf" \
  -F "Resumes=@resume2.docx"
```

**Response:** `201 Created`
```json
{
  "value": "870e8400-e29b-41d4-a716-446655440400",
  "message": "Resume import initiated successfully"
}
```

**Response Fields:**

| Field | Type | Description |
|-------|------|-------------|
| `value` | UUID | The ID of the resume import batch |
| `message` | String | Success message |

**Preconditions:**
- Campaign must exist
- At least one valid resume file must be provided

**Processing:**
- Resume parsing is asynchronous
- Status can be checked via separate status endpoints
- Parsed candidate applications are created from resumes

**Possible Errors:**
- `400 Bad Request` - No resumes provided or invalid file format
- `404 Not Found` - Campaign not found
- `413 Payload Too Large` - Total upload size exceeds limit

---

## Assessment Strategy Management

### 9. Create Assessment Strategy

**Endpoint:** `POST /api/campaigns/{campaignId}/assessment-strategy`

**Summary:** Generates an assessment strategy for a campaign.

**Description:** Generates an AI-suggested assessment strategy using the approved job profile and hiring strategy. This defines the questions, competency evaluation criteria, and scoring methodology for candidate assessments.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `campaignId` | UUID | Yes | The campaign ID for which to generate the assessment strategy |

**Request Body:** Empty (no body required)

**Response:** `201 Created`
```json
{
  "value": "980e8400-e29b-41d4-a716-446655440500",
  "message": "Assessment strategy created successfully"
}
```

**Response Fields:**

| Field | Type | Description |
|-------|------|-------------|
| `value` | UUID | The ID of the newly created assessment strategy |
| `message` | String | Success message |

**Preconditions:**
- Campaign must exist
- Job profile must be approved
- Hiring strategy must be approved

**Possible Errors:**
- `400 Bad Request` - Campaign in invalid state
- `404 Not Found` - Campaign, job profile, or hiring strategy not found
- `409 Conflict` - Assessment strategy already exists
- `420 Enhance Your Calm` - AI service rate limit exceeded
- `422 Unprocessable Entity` - Missing required approved strategies

**Produces:**
- `201 Created` - Returns UUID of created strategy
- `400 Bad Request` - Validation error
- `404 Not Found` - Campaign not found
- `409 Conflict` - Conflict during creation

---

### 10. Approve Assessment Strategy

**Endpoint:** `POST /api/campaigns/assessment-strategies/{assessmentStrategyId}/approve`

**Summary:** Approves an assessment strategy.

**Description:** Marks a generated assessment strategy as approved, making it ready for use by TAO Assess (the assessment execution module).

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `assessmentStrategyId` | UUID | Yes | The ID of the assessment strategy to approve |

**Request Body:**
```json
{
  "approvedByUserId": "950e8400-e29b-41d4-a716-446655440003"
}
```

**Request Fields:**

| Field | Type | Required | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `approvedByUserId` | UUID | Yes | The user ID of the approver | Must be a valid UUID of an existing user |

**Response:** `204 No Content`

No response body is returned.

**Produces:**
- `204 No Content` - Approval successful
- `400 Bad Request` - Validation error
- `404 Not Found` - Assessment strategy not found

---

## Enums & Status Values

### Campaign Status
Used in campaign workflow to track the campaign lifecycle.

| Value | Name | Description |
|-------|------|-------------|
| 1 | `Ready` | Campaign created but not yet opened for applications |
| 2 | `Open` | Campaign is actively accepting candidate applications |
| 3 | `Closed` | Campaign is no longer accepting applications |
| 4 | `Archived` | Campaign has been archived |

### Job Profile Status
Indicates the approval state of a job profile.

| Value | Name | Description |
|-------|------|-------------|
| 1 | `Generated` | Job profile has been AI-generated but not yet approved |
| 2 | `Approved` | Job profile has been approved by hiring manager |

### Hiring Strategy Status
Indicates the approval state of a hiring strategy.

| Value | Name | Description |
|-------|------|-------------|
| 1 | `Generated` | Hiring strategy has been AI-generated but not yet approved |
| 2 | `Approved` | Hiring strategy has been approved for use |

### Assessment Strategy Status
Indicates the approval state of an assessment strategy.

| Value | Name | Description |
|-------|------|-------------|
| 1 | `Generated` | Assessment strategy has been AI-generated but not yet approved |
| 2 | `Approved` | Assessment strategy has been approved for use by assessments |

---

## Error Handling

### Standard Error Response Format

All error responses follow this format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-1234567890abcdef1234567890abcdef-1234567890abcdef-00",
  "errors": {
	"fieldName": ["Error message 1", "Error message 2"]
  }
}
```

### Common HTTP Status Codes

| Status Code | Meaning | Common Causes |
|-------------|---------|---------------|
| `200 OK` | Request successful | GET operations completed |
| `201 Created` | Resource created | POST operations with successful creation |
| `204 No Content` | Request successful, no content | Update/Approval operations |
| `400 Bad Request` | Invalid request data | Missing/invalid fields, validation errors |
| `404 Not Found` | Resource not found | Non-existent campaign/profile/strategy ID |
| `409 Conflict` | Conflict detected | Duplicate data, invalid state transitions |
| `413 Payload Too Large` | Request too large | File upload exceeds limits |
| `420 Enhance Your Calm` | Rate limit exceeded | Too many AI generation requests |
| `422 Unprocessable Entity` | Validation failed | Business logic validation errors |
| `500 Internal Server Error` | Server error | Unhandled exceptions, AI service failures |

### Validation Error Example

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-abc123def456-ghi789-00",
  "errors": {
	"name": ["The name field is required."],
	"numberOfOpenings": ["The field NumberOfOpenings must be between 1 and 1000."]
  }
}
```

---

## Request/Response Examples

### Complete Campaign Creation Workflow Example

**Step 1: Create Campaign**
```bash
curl -X POST http://api.example.com/api/campaigns/ \
  -H "Content-Type: application/json" \
  -d '{
	"organizationId": "550e8400-e29b-41d4-a716-446655440000",
	"name": "Senior Software Engineer - Q1 2024",
	"referenceNumber": "CAMPAIGN-2024-001",
	"recruiterId": "750e8400-e29b-41d4-a716-446655440001",
	"hiringManagerId": "850e8400-e29b-41d4-a716-446655440002",
	"numberOfOpenings": 3
  }'

# Response
{
  "value": "550e8400-e29b-41d4-a716-446655440100",
  "message": "Campaign created successfully"
}
```

**Step 2: Create Job Profile**
```bash
curl -X POST http://api.example.com/api/campaigns/550e8400-e29b-41d4-a716-446655440100/job-profile \
  -H "Content-Type: application/json" \
  -d '{
	"originalJobDescription": "We are looking for a Senior Software Engineer with 5+ years of experience in cloud-native applications using Kubernetes, Docker, and microservices architecture. Must have experience with CI/CD pipelines and DevOps practices."
  }'

# Response
{
  "value": "660e8400-e29b-41d4-a716-446655440200",
  "message": "Job profile generated successfully"
}
```

**Step 3: Approve Job Profile**
```bash
curl -X POST http://api.example.com/api/jobprofiles/660e8400-e29b-41d4-a716-446655440200/approve \
  -H "Content-Type: application/json" \
  -d '{
	"approvedByUserId": "950e8400-e29b-41d4-a716-446655440003"
  }'

# Response: 204 No Content
```

**Step 4: Create Hiring Strategy**
```bash
curl -X POST http://api.example.com/api/campaigns/550e8400-e29b-41d4-a716-446655440100/hiring-strategy \
  -H "Content-Type: application/json"

# Response
{
  "value": "760e8400-e29b-41d4-a716-446655440300",
  "message": "Hiring strategy generated successfully"
}
```

**Step 5: Get Hiring Strategy**
```bash
curl -X GET http://api.example.com/api/campaigns/550e8400-e29b-41d4-a716-446655440100/hiring-strategy

# Response
{
  "value": {
	"id": "760e8400-e29b-41d4-a716-446655440300",
	"campaignId": "550e8400-e29b-41d4-a716-446655440100",
	"generatedContent": "Recommended sourcing channels: LinkedIn, Technical job boards, Referral programs. Evaluation focus: Cloud architecture experience, Kubernetes expertise, CI/CD pipeline knowledge.",
	"structuredContent": "{\"sourcing_strategies\": [...], \"evaluation_criteria\": [...]}",
	"status": 1,
	"providerName": "OpenAI",
	"modelName": "gpt-4",
	"promptVersion": 1,
	"createdOnUtc": "2024-01-15T11:00:00Z"
  },
  "message": "Hiring strategy retrieved successfully"
}
```

**Step 6: Approve Hiring Strategy**
```bash
curl -X POST http://api.example.com/api/campaigns/760e8400-e29b-41d4-a716-446655440300/approve \
  -H "Content-Type: application/json" \
  -d '{
	"approvedByUserId": "950e8400-e29b-41d4-a716-446655440003"
  }'

# Response: 204 No Content
```

**Step 7: Create Assessment Strategy**
```bash
curl -X POST http://api.example.com/api/campaigns/550e8400-e29b-41d4-a716-446655440100/assessment-strategy \
  -H "Content-Type: application/json"

# Response
{
  "value": "980e8400-e29b-41d4-a716-446655440500",
  "message": "Assessment strategy created successfully"
}
```

**Step 8: Approve Assessment Strategy**
```bash
curl -X POST http://api.example.com/api/campaigns/assessment-strategies/980e8400-e29b-41d4-a716-446655440500/approve \
  -H "Content-Type: application/json" \
  -d '{
	"approvedByUserId": "950e8400-e29b-41d4-a716-446655440003"
  }'

# Response: 204 No Content
```

**Step 9: Import Resumes**
```bash
curl -X POST http://api.example.com/api/campaigns/550e8400-e29b-41d4-a716-446655440100/resume-imports \
  -F "Resumes=@candidate1.pdf" \
  -F "Resumes=@candidate2.docx" \
  -F "Resumes=@candidate3.pdf"

# Response
{
  "value": "870e8400-e29b-41d4-a716-446655440400",
  "message": "Resume import initiated successfully"
}
```

---

## Notes for UI Developers

1. **Asynchronous Operations**: Job Profile, Hiring Strategy, and Assessment Strategy generation are asynchronous operations. The endpoint returns a resource ID immediately, but generation may still be in progress.

2. **Status Checks**: Use the GET endpoints (e.g., GET `/api/jobprofiles/{jobProfileId}`) to check the current status and retrieve generated content once available.

3. **Approval Workflow**: All AI-generated content (Job Profiles, Hiring Strategies, Assessment Strategies) must be approved before they can be used in subsequent steps.

4. **UUID Format**: All IDs use UUID (v4) format. Ensure proper validation in your client.

5. **DateTime Format**: All timestamps use ISO 8601 format in UTC (e.g., `2024-01-15T10:30:00Z`).

6. **File Uploads**: Resume imports use multipart/form-data. Ensure your upload implementation handles multiple files correctly.

7. **Error Handling**: Always check the HTTP status code and error response structure. Provide meaningful error messages to end users based on the error codes.

8. **Rate Limiting**: AI generation operations have rate limits. Implement exponential backoff retry logic (wait and retry after appropriate interval) when receiving 420 responses.

9. **Validation**: Perform client-side validation for string lengths, UUID formats, and numeric ranges to improve user experience before sending requests.

10. **Campaign Flow**: The typical flow is:
	- Create Campaign
	- Create & Approve Job Profile
	- Create & Approve Hiring Strategy
	- Create & Approve Assessment Strategy
	- Import Resumes (can happen at any point after campaign creation)

---

**Document Version:** 1.0  
**Last Updated:** 2024-01-15  
**API Stability:** Production-Ready
