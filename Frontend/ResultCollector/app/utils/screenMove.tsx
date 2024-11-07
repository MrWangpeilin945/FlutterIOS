import type { NavigateFunction } from 'react-router-dom';

export const createNavigate = (navigate: NavigateFunction) => {
  return {
    navigateProgress: () => {
      navigate("/progress");
    },
    navigateHome: () => {
      navigate("/");
    },
    navigateBack: () => {
      navigate(-1);
    },
    navigateExamMenu: () => {
      navigate("");
    }
  };
};

export type screenMove = ReturnType<typeof createNavigate>;