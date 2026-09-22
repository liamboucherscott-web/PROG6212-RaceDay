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

