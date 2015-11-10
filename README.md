# Cascade.CBCA.Member
Creates a MemberPart and adds it to the UserPart. This extends the User with information about CBCA Membership.

All fields are stored as columns as well as infosets, ensuring that data can be searched, sorted, 
etc with standard SQL queries. This is a requirement for reporting.

The Memberpart is kept separate from the UserPart for security reasons. This makes it easy to make Member 
info available for reporting without compromising the identity data help in the User table. Certain fields
(name, email address) are duplicated for the same reason.