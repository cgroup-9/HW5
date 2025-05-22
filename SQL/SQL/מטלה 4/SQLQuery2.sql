EXEC SP_InsertUser @name='Eden', @email='eden@example.com', @password='Password123';

SELECT * FROM UsersTable 

EXEC SP_InsertUser @name='RON', @email='RON@example.com', @password='assword123';

SELECT * FROM UsersTable WHERE name = 'mika';
