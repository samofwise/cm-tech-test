import { arrangeBy } from './arrangeBy';

describe('arrangeBy', () => {
  const users = [
    {
      id: 1,
      name: 'bob',
    },
    {
      id: 2,
      name: 'sally',
    },
    {
      id: 3,
      name: 'bob',
      age: 30,
    },
  ];

  it('should group objects by the specified key', () => {
    const arrangeByName = arrangeBy('name');
    const result = arrangeByName(users);

    expect(result).toEqual({
      bob: [
        {
          id: 1,
          name: 'bob',
        },
        {
          id: 3,
          name: 'bob',
          age: 30,
        },
      ],
      sally: [
        {
          id: 2,
          name: 'sally',
        },
      ],
    });
  });

  it('should not mutate the original array', () => {
    const arrangeByName = arrangeBy('name');
    const originalUsers = [...users];
    arrangeByName(users);

    expect(users).toEqual(originalUsers);
  });

  it('should not mutate the objects in the array', () => {
    const arrangeByName = arrangeBy('name');
    const originalUser = { ...users[0] };
    arrangeByName(users);

    expect(users[0]).toEqual(originalUser);
  });

  it('should exclude objects without the specified key', () => {
    const usersWithMissingKey = [
      ...users,
      { id: 4, age: 25 }, // missing 'name' key
    ];
    const arrangeByName = arrangeBy('name');
    const result = arrangeByName(usersWithMissingKey);

    expect(result).toEqual({
      bob: [
        {
          id: 1,
          name: 'bob',
        },
        {
          id: 3,
          name: 'bob',
          age: 30,
        },
      ],
      sally: [
        {
          id: 2,
          name: 'sally',
        },
      ],
    });
  });

  it('should exclude null/undefined values', () => {
    const usersWithNull = [
      ...users,
      null,
      undefined,
    ];
    const arrangeByName = arrangeBy('name');
    const result = arrangeByName(usersWithNull);

    expect(result).toEqual({
      bob: [
        {
          id: 1,
          name: 'bob',
        },
        {
          id: 3,
          name: 'bob',
          age: 30,
        },
      ],
      sally: [
        {
          id: 2,
          name: 'sally',
        },
      ],
    });
  });

  it('should exclude non-object values', () => {
    const usersWithNonObjects = [
      ...users,
      'not an object',
      123,
      true,
    ];
    const arrangeByName = arrangeBy('name');
    const result = arrangeByName(usersWithNonObjects);

    expect(result).toEqual({
      bob: [
        {
          id: 1,
          name: 'bob',
        },
        {
          id: 3,
          name: 'bob',
          age: 30,
        },
      ],
      sally: [
        {
          id: 2,
          name: 'sally',
        },
      ],
    });
  });

  it('should handle empty arrays', () => {
    const arrangeByName = arrangeBy('name');
    const result = arrangeByName([]);

    expect(result).toEqual({});
  });

  it('should handle arrays with only invalid elements', () => {
    const arrangeByName = arrangeBy('name');
    const result = arrangeByName([null, undefined, 'not an object', 123]);

    expect(result).toEqual({});
  });

  it('should handle multiple keys', () => {
    const arrangeById = arrangeBy('id');
    const result = arrangeById(users);

    expect(result).toEqual({
      1: [{ id: 1, name: 'bob' }],
      2: [{ id: 2, name: 'sally' }],
      3: [{ id: 3, name: 'bob', age: 30 }],
    });
  });


  it('should handle objects with undefined/null key values', () => {
    const usersWithNullValues = [
      ...users,
      { id: 4, name: null },
      { id: 5, name: undefined },
    ];
    const arrangeByName = arrangeBy('name');
    const result = arrangeByName(usersWithNullValues);

    expect(result).toEqual({
      bob: [
        { id: 1, name: 'bob' },
        { id: 3, name: 'bob', age: 30 },
      ],
      sally: [
        { id: 2, name: 'sally' },
      ],
      null: [{ id: 4, name: null }],
      undefined: [{ id: 5, name: undefined }],
    });
  });

  it('should handle objects with non-primitive key values', () => {
    const usersWithObjects = [
      { id: 1, name: { first: 'bob' } },
      { id: 2, name: { first: 'sally' } },
      { id: 3, name: { first: 'bob' } },
    ];
    const arrangeByName = arrangeBy('name');
    const result = arrangeByName(usersWithObjects);

    expect(result).toEqual({
      '[object Object]': usersWithObjects,
    });
  });

  it('should handle objects with empty string as key value', () => {
    const usersWithEmpty = [
      ...users,
      { id: 4, name: '' },
    ];
    const arrangeByName = arrangeBy('name');
    const result = arrangeByName(usersWithEmpty);

    console.log('TESTING', result);

    expect(result).toEqual({
      '': [{ id: 4, name: '' }],
      bob: [
        { id: 1, name: 'bob' },
        { id: 3, name: 'bob', age: 30 },
      ],
      sally: [
        { id: 2, name: 'sally' },
      ],
    });
  });

  it('should handle function currying correctly', () => {
    const arrangeByName = arrangeBy('name');
    const result1 = arrangeByName(users);
    const result2 = arrangeByName(users);

    expect(result1).toEqual(result2);
    expect(arrangeByName).toBeInstanceOf(Function);
  });
}); 