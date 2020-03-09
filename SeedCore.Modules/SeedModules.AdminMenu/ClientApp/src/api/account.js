export const logout = () => {
  window.location = `/SeedModules.Account/Account/Logout?returnUrl=${window.location}`;
};
