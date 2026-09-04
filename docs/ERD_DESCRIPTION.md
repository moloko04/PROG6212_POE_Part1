ERD Description

Entities (6)
1. User- Stores all users with roles (Organiser, Participant, Admin)
2. Event- Stores event details
3. Category- Stores event categories
4. Enrolment- Links participants to events
5. Result- Stores participant results
6. Prize- Stores prize details

Relationships
- User (Organiser) → Event: One-to-Many
- Event → Category: One-to-Many
- Event → Enrolment: One-to-Many
- Event → Prize: One-to-Many
- Enrolment → Result: One-to-One
