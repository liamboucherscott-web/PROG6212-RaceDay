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

