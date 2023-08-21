
using Prototype.Data;
using Prototype.Misc;
using Prototype.Model;
using Prototype.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Controller
{
    public class ChooseBuffController
    {

        public ChooseBuffController(
            AppData appData,
            PlayerModel player,
            GameEventService gameEventService,
            AppEventService appEventService)
        {
            _appData = appData;
            _player = player;
            _gameBalance = _appData.GameBalance;
            _gameEventService = gameEventService;
            _appEventService = appEventService;

            _gameEventService.PlayerLevelChanged += HandlePlayerLevelChanged;
        }

        ~ChooseBuffController()
        {
            _gameEventService.PlayerLevelChanged -= HandlePlayerLevelChanged;
        }

        private void HandlePlayerLevelChanged(object sender, GameEventService.PlayerLevelChangedEventArgs e)
        {
            if (e.IsInit)
            {
                return;
            }

            _appEventService.OnGamePause();

            List<BuffConfig> availableBuffsPool = new List<BuffConfig>();

            foreach (BuffConfig buffCfg in _gameBalance.BuffConfigs)
            {
                if (_player.AppliedBuffs.Contains(buffCfg.Id))
                {
                    continue;
                }

                if (buffCfg.BuffType == BuffTypeEnum.IncreaseDamage)
                {
                    var buffIdOfThisTypeInPlayer = _player.AppliedBuffs.FirstOrDefault(buffIdInPlayer =>
                    {
                        var buffCfgInPlayer = _gameBalance.BuffConfigs.FirstOrDefault(_ => _.Id.Equals(buffIdInPlayer));
                        if (buffCfgInPlayer.BuffType == buffCfg.BuffType
                            && buffCfgInPlayer.DamageType == buffCfg.DamageType)
                        {
                            return true;
                        }

                        return false;
                    });

                    BuffConfig buffCfgOfThisTypeInPlayer = _gameBalance.BuffConfigs.FirstOrDefault(_ => _.Id.Equals(buffIdOfThisTypeInPlayer));

                    if (buffCfgOfThisTypeInPlayer == default && buffCfg.BuffLevel == 1
                        || buffCfgOfThisTypeInPlayer != default && buffCfgOfThisTypeInPlayer.BuffLevel == buffCfg.BuffLevel - 1)
                    {
                        availableBuffsPool.Add(buffCfg);
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (buffCfg.BuffType == BuffTypeEnum.IncreaseMaxHealth ||
                    buffCfg.BuffType == BuffTypeEnum.IncreaseMovementSpeed)
                {
                    var buffIdOfThisTypeInPlayer = _player.AppliedBuffs.FirstOrDefault(buffIdInPlayer =>
                    {
                        var buffCfgInPlayer = _gameBalance.BuffConfigs.FirstOrDefault(_ => _.Id.Equals(buffIdInPlayer));
                        if (buffCfgInPlayer.BuffType == buffCfg.BuffType)
                        {
                            return true;
                        }

                        return false;
                    });

                    BuffConfig buffCfgOfThisTypeInPlayer = _gameBalance.BuffConfigs.FirstOrDefault(_ => _.Id.Equals(buffIdOfThisTypeInPlayer));

                    if (buffCfgOfThisTypeInPlayer == default && buffCfg.BuffLevel == 1
                        || buffCfgOfThisTypeInPlayer != default && buffCfgOfThisTypeInPlayer.BuffLevel == buffCfg.BuffLevel - 1)
                    {
                        availableBuffsPool.Add(buffCfg);
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }

                availableBuffsPool.Add(buffCfg);

            }

            BuffConfig[] chosenBuffs = new BuffConfig[2];
            Random random = new Random();

            WeightedRandomSelector<BuffConfig> randomSelector = new WeightedRandomSelector<BuffConfig>(availableBuffsPool);
            chosenBuffs[0] = randomSelector.GetRandomElement();
            availableBuffsPool.Remove(chosenBuffs[0]);

            if (chosenBuffs[0].BuffType == BuffTypeEnum.Heal)
            {
                for (int i = 0; i < availableBuffsPool.Count; ++i)
                {
                    if (availableBuffsPool[i].BuffType == BuffTypeEnum.Heal)
                    {
                        availableBuffsPool.RemoveAt(i);
                        --i;
                    }
                }
            }

            randomSelector = new WeightedRandomSelector<BuffConfig>(availableBuffsPool);
            chosenBuffs[1] = randomSelector.GetRandomElement();

            _gameEventService.OnChooseBuffWindowOpen(new GameEventService.ChooseBuffWindowOpenEventArgs { AvailableBuffs = chosenBuffs });
        }

        // Injected
        private AppData _appData;
        private PlayerModel _player;
        private GameEventService _gameEventService;
        private AppEventService _appEventService;
        //
        private GameBalanceData _gameBalance;
    }
}
