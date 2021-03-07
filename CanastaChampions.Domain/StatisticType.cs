﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.Domain
{
    public enum StatisticType
    {
        // Competition
        MOST_GAMES_IN_A_DAY,
        MOST_DEAL_BONUSES_IN_A_COMPETITION,
        MOST_GAME_WINS_IN_A_COMPETITION,
        MOST_ROUND_WINS_IN_A_COMPETITION,
        MOST_GAME_LOSSES_IN_A_COMPETITION,
        MOST_ROUND_LOSSES_IN_A_COMPETITION,

        // Round
        FASTEST_ROUND,
        HIGHEST_SCORE_IN_A_ROUND,
        LOWEST_SCORE_IN_A_ROUND,
        MOST_CANASTAS_IN_A_ROUND,
        MOST_NATURAL_CANASTAS_IN_A_ROUND,
        MOST_UNNATURAL_CANASTAS_IN_A_ROUND,
        MOST_PENALTIES_IN_A_ROUND,
        MOST_AWARDS_IN_A_ROUND,

        // Game
        FASTEST_GAME,
        HIGHEST_SCORE_IN_A_GAME,
        LOWEST_SCORE_IN_A_GAME,
        MOST_CANASTAS_IN_A_GAME,
        MOST_NATURAL_CANASTAS_IN_A_GAME,
        MOST_UNNATURAL_CANASTAS_IN_A_GAME,
        MOST_PENALTIES_IN_A_GAME,
        MOST_POSITIVE_FOUR_RED_THREES_IN_A_GAME,
        MOST_NEGATIVE_FOUR_RED_THREES_IN_A_GAME,
        MOST_NEGATIVE_CONSECUTIVE_SCORES_IN_A_GAME,
        MOST_POSITIVE_CONSECUTIVE_SCORES_IN_A_GAME,
        MOST_NEGATIVE_SCORES_IN_A_GAME,
        MOST_POSITIVE_SCORES_IN_A_GAME,
        HIGHEST_NUMBER_OF_ROUNDS_IN_A_GAME,
        LOWEST_NUMBER_OF_ROUNDS_IN_A_GAME,
        MOST_DEAL_BONUSES_IN_A_GAME,
        MOST_AWARDS_IN_A_GAME,
        MOST_WINS_IN_A_GAME,
        MOST_NUMBER_OF_INCOMPLETES_IN_A_GAME,
    }
}