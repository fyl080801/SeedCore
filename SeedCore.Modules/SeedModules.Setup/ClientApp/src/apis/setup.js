import cookie from 'react-cookies';

export const getDatabaseProviders = async () => {
  const response = await fetch(
    '/SeedModules.Setup/Settings/DatabaseProviders',
    {
      method: 'GET'
    }
  );
  return await response.json();
};

export const getTimeZones = async () => {
  const response = await fetch('/SeedModules.Setup/Settings/TimeZones', {
    method: 'GET'
  });
  return await response.json();
};

export const getRecipes = async () => {
  const response = await fetch('/SeedModules.Setup/Settings/Recipes', {
    method: 'GET'
  });
  return await response.json();
};

export const postExecute = async data => {
  const response = await fetch('/SeedModules.Setup/Setup/Execute', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json; charset=utf-8',
      [cookie.load('XSRF-HEAD')]: cookie.load('XSRF-TOKEN')
    },
    body: JSON.stringify(data)
  });
  return await response.json();
};
