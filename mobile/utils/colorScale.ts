import { COLORS } from '../constants/theme';

/**
 * Gets the color for a specific rank.
 * @param rank The rank of the guess
 * @param colorCategory The pre-calculated color category string (hot/warm/cool/cold)
 */
export const getColorForCategory = (colorCategory: string): string => {
  switch (colorCategory) {
    case 'hot':
      return COLORS.rankHot;
    case 'warm':
      return COLORS.rankWarm;
    case 'cool':
      return COLORS.rankCool;
    case 'cold':
      return COLORS.rankCold;
    default:
      return COLORS.rankCold;
  }
};

/**
 * Gets a dynamic color directly from rank if category isn't provided
 */
export const getColorForRank = (rank: number): string => {
  if (rank <= 100) return COLORS.rankHot;
  if (rank <= 500) return COLORS.rankWarm;
  if (rank <= 1000) return COLORS.rankCool;
  return COLORS.rankCold;
};
