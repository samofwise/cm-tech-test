export const arrangeBy = (key: string) => (array: any[]) => {
  return array.reduce((acc, item) => {
    if (item === null || item === undefined || typeof item !== 'object') return acc;
    if (!(key in item)) return acc;

    const value = item[key];

    if (!acc[value]) acc[value] = [];

    acc[value].push(item);
    return acc;
  }, {});
};


