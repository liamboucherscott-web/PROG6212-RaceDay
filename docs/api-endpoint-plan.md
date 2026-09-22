\# RaceDay — API Endpoint Plan



\*\*Module:\*\* PROG6212 - Programming 2B

\*\*Project:\*\* RaceDay Event Management System

\*\*Part:\*\* 1 — System Planning



\---



\## Overview



This document defines every REST API endpoint the RaceDay backend will expose. It is the specification for the Part 2 ASP.NET Core Web API implementation.



\### Conventions



| Column | Meaning |

|---|---|

| \*\*HTTP Method\*\* | GET (read), POST (create), PUT (update), DELETE (remove) |

| \*\*Route\*\* | The URL path. All routes are prefixed with `/api/`. |

| \*\*Description\*\* | One sentence explaining what the endpoint does. |

| \*\*Role Required\*\* | `None` (public), `Any` (any logged-in user), `Organiser`, or `Participant`. |

| \*\*Request Body\*\* | JSON fields expected in the request body. `None` for GET and DELETE. |

| \*\*Expected Response\*\* | HTTP status codes for both success and failure cases. |



\### Roles



\- \*\*Organiser\*\* — creates and manages events, categories, and captures results.

\- \*\*Participant\*\* — browses events, enrols in categories, and views personal results.



\### Summary



| Resource Group | Endpoints |

|---|---|

| Authentication | 4 |

| User Profile | 3 |

| Events | 6 |

| Categories | 5 |

| Event Enrolments | 5 |

| Results | 5 |

| \*\*Total\*\* | \*\*28\*\* |


---

## 1. Authentication

These endpoints handle registration, login, logout, and getting the current user. Register and login are the only endpoints anyone can call without being logged in.

| Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | `/api/auth/register` | Creates a new user account and lets them pick Organiser or Participant. | None | `{ firstName, lastName, email, password, phoneNumber, roleId }` | 201 Created — returns the new user. 400 Bad Request — missing or invalid fields. 409 Conflict — email already in use. |
| POST | `/api/auth/login` | Logs a user in and starts a session so the API remembers who they are. | None | `{ email, password }` | 200 OK — returns user info and role. 400 Bad Request — fields missing. 401 Unauthorized — wrong email or password. |
| POST | `/api/auth/logout` | Ends the user's session. | Any | None | 200 OK — session cleared. 401 Unauthorized — not logged in. |
| GET | `/api/auth/me` | Returns the details of whoever is currently logged in. | Any | None | 200 OK — current user info. 401 Unauthorized — not logged in. |


---

## 2. User Profile

Both Organisers and Participants can view and update their own profile. They can also upload a profile picture, which we will store in Azure Blob Storage later in Part 3.

| Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | `/api/users/{id}` | Gets a single user's profile by their ID. Users can only view their own profile. | Any (self only) | None | 200 OK — returns the user. 403 Forbidden — trying to view someone else's profile. 404 Not Found — user doesn't exist. |
| PUT | `/api/users/{id}` | Updates the logged-in user's own profile info. | Any (self only) | `{ firstName, lastName, phoneNumber, profileImageUrl }` | 200 OK — updated user. 400 Bad Request — invalid data. 403 Forbidden — trying to edit someone else. |
| POST | `/api/users/{id}/profile-image` | Uploads a profile picture. The file goes to Azure Blob Storage via the API. | Any (self only) | `multipart/form-data` | 200 OK — returns the image URL. 400 Bad Request — no file or wrong format. 403 Forbidden — trying to upload for someone else. |

---

## 3. Events

Events are created and managed by Organisers. Both Organisers and Participants can browse and view events. Each event has a name, description, date, location, distance, and an event type (Run, Walk, or Cycle).

| Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | `/api/events` | Returns a list of all events. Can be filtered by event type or date. | Any | None | 200 OK — array of events. |
| GET | `/api/events/{id}` | Returns the full details of one event. | Any | None | 200 OK — event details. 404 Not Found — event doesn't exist. |
| POST | `/api/events` | Creates a new event. Only Organisers can do this, and the event is linked to them. | Organiser | `{ name, description, eventDate, location, distance, eventTypeId }` | 201 Created — new event. 400 Bad Request — validation error. 401 Unauthorized — not logged in. 403 Forbidden — not an Organiser. |
| PUT | `/api/events/{id}` | Updates an event. Only the Organiser who created it can edit it. | Organiser | `{ name, description, eventDate, location, distance, eventTypeId }` | 200 OK — updated event. 403 Forbidden — not the owner. 404 Not Found — event doesn't exist. |
| DELETE | `/api/events/{id}` | Deletes an event. Only the owning Organiser can delete it. | Organiser | None | 204 No Content — deleted. 403 Forbidden — not the owner. 404 Not Found — event doesn't exist. |
| POST | `/api/events/{id}/banner` | Uploads a banner image for an event. Stored in Azure Blob Storage (Part 3). | Organiser | `multipart/form-data` | 200 OK — image URL. 400 Bad Request — no file or wrong format. 403 Forbidden — not the owner. |

---

## 4. Categories

Categories are age or distance groups for an event (like "Under 20", "Senior", "10km", "21km"). Organisers add them to their own events. Both roles can view available categories.

| Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | `/api/events/{eventId}/categories` | Returns a list of all categories for a specific event. | Any | None | 200 OK — array of categories. 404 Not Found — event doesn't exist. |
| GET | `/api/categories/{id}` | Returns the details of one category. | Any | None | 200 OK — category details. 404 Not Found — category doesn't exist. |
| POST | `/api/events/{eventId}/categories` | Adds a new category to an event. Only the owning Organiser can do this. | Organiser | `{ name, description, maxParticipants, entryFee }` | 201 Created — new category. 400 Bad Request — invalid data. 403 Forbidden — not the event owner. |
| PUT | `/api/categories/{id}` | Updates a category. Only the owning Organiser can edit it. | Organiser | `{ name, description, maxParticipants, entryFee }` | 200 OK — updated category. 403 Forbidden — not the owner. 404 Not Found — category doesn't exist. |
| DELETE | `/api/categories/{id}` | Deletes a category. Only the owning Organiser can delete it. | Organiser | None | 204 No Content — deleted. 403 Forbidden — not the owner. 404 Not Found — category doesn't exist. |

---

## 5. Event Enrolments

A Participant enrols in an event by choosing a category. This creates an Enrolment that links the Participant, the Event, and the Category together. Organisers can view all enrolments for their own events.

| Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | `/api/enrolments` | Enrols the logged-in Participant in an event by selecting a category. | Participant | `{ eventId, categoryId }` | 201 Created — new enrolment. 400 Bad Request — invalid data. 403 Forbidden — not a Participant. 409 Conflict — already enrolled in this event and category. |
| GET | `/api/enrolments/my` | Returns all enrolments belonging to the logged-in Participant. | Participant | None | 200 OK — array of enrolments. 401 Unauthorized — not logged in. |
| GET | `/api/enrolments/{id}` | Returns a single enrolment. Only the participant who made it or the event's Organiser can view it. | Any (owner or Organiser) | None | 200 OK — enrolment details. 403 Forbidden — not allowed. 404 Not Found — enrolment doesn't exist. |
| DELETE | `/api/enrolments/{id}` | Cancels the Participant's own enrolment. | Participant | None | 204 No Content — cancelled. 403 Forbidden — not the owner. 404 Not Found — enrolment doesn't exist. |
| GET | `/api/events/{eventId}/enrolments` | Returns a list of all enrolments for an event. Only the owning Organiser can view this. | Organiser | None | 200 OK — array of enrolments. 403 Forbidden — not the event owner. 404 Not Found — event doesn't exist. |

---

## 6. Results

After an event finishes, the Organiser captures each participant's finish time and finishing position. Participants can then view their own results as part of their race history.

| Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | `/api/enrolments/{enrolmentId}/result` | Captures a result for an enrolment. Only the owning Organiser can do this. | Organiser | `{ finishTime, finishingPosition }` | 201 Created — new result. 400 Bad Request — invalid data. 403 Forbidden — not the event owner. 404 Not Found — enrolment doesn't exist. 409 Conflict — result already captured. |
| PUT | `/api/results/{id}` | Updates an existing result. Only the owning Organiser can edit it. | Organiser | `{ finishTime, finishingPosition }` | 200 OK — updated result. 403 Forbidden — not the event owner. 404 Not Found — result doesn't exist. |
| GET | `/api/results/my` | Returns all results belonging to the logged-in Participant. | Participant | None | 200 OK — array of results with event name, date, category, finish time, and position. 401 Unauthorized — not logged in. |
| GET | `/api/results/{id}` | Returns a single result. Only the owning Participant or the event's Organiser can view it. | Any (owner or Organiser) | None | 200 OK — result details. 403 Forbidden — not allowed. 404 Not Found — result doesn't exist. |
| GET | `/api/events/{eventId}/results` | Returns all results for an event. Only the owning Organiser can view this. | Organiser | None | 200 OK — array of results. 403 Forbidden — not the event owner. 404 Not Found — event doesn't exist. |

---

## Endpoint Plan Complete

Total endpoints planned: **28**

This plan is the specification for the Part 2 ASP.NET Core Web API implementation. Any deviation from this plan in the final implementation must be documented in the README.

