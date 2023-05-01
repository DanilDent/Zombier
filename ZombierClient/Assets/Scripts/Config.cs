using Prototype.Data;
using Prototype.Extensions;
using System.Linq;

namespace Prototype
{
    public static class Config
    {
        public const string TAG_PFAB_LOCATION_0_LEVEL_0 = "Location0Level0";
        public const string TAG_PFAB_PLAYER = "PlayerYbot";
        public const string TAG_PFAB_PISTOL = "pistol";
        public const string TAG_PFAB_ENEMY_SIMPLE = "enemy.simple";
        public const string TAG_PFAB_PROJECTILE_PISTOL = "projectile.pistol";

        public static readonly IdData ID_PLAYER = new IdData("id_player");
        public static readonly IdData ID_PISTOL = new IdData("id_pistol");
        public static readonly IdData ID_PROJECTILE_PISTOL = new IdData("id_meta_projectile_pistol");
        public static readonly IdData ID_LOCATION_0 = new IdData("id_meta_location_0");
        public static readonly IdData ID_ENEMY_SIMPLE = new IdData("id_meta_enemy_simple");

        public static void CreateGameMetaData(out GameMetaData target)
        {
            var model = new GameMetaData();
            model.Data = new SharedData();
            var container = model.Data;
            container.Player = new PlayerData
            {
                IdMeta = ID_PLAYER,
                IdUser = ID_PLAYER.GetIdUser(),
                IdSession = ID_PLAYER.GetIdSession(),
                ResourceTag = TAG_PFAB_PLAYER,
                //
                MaxHealth = 100,
                Health = 100,
                Speed = 10.0f,
            };

            AddWeapon(new WeaponData
            {
                IdMeta = ID_PISTOL,
                IdUser = ID_PISTOL.GetIdUser(),
                IdSession = ID_PISTOL.GetIdSession(),
                ResourceTag = TAG_PFAB_PISTOL,
                //
                AttackRange = 7f,
                FireRateRPM = 18f,
                IdProjectile = ID_PROJECTILE_PISTOL,
            });

            AddProjectile(new ProjectileData
            {
                IdMeta = ID_PROJECTILE_PISTOL,
                ResourceTag = TAG_PFAB_PROJECTILE_PISTOL,
                //
                Speed = 80,
                Damage = 1,
            });

            AddLocation(new LocationData
            {
                IdMeta = ID_LOCATION_0,
                IdUser = ID_LOCATION_0.GetIdUser(),
                IdSession = ID_LOCATION_0.GetIdSession(),
                ResourceTag = string.Empty,
                //
                Levels = new[]
                {
                    new LevelData
                    {
                        ResourceTag = TAG_PFAB_LOCATION_0_LEVEL_0
                    }
                }
            });

            AddEnemy(new EnemyData
            {
                IdMeta = ID_ENEMY_SIMPLE,
                ResourceTag = TAG_PFAB_ENEMY_SIMPLE,
                //
                Health = 30,
                MaxHealth = 30,
                Speed = 2f,
                ChaseRange = 10f,
                AttackRange = 2.5f,
                AttackInterval = 5f,
                Damage = 1,
            });

            target = model;

            void AddWeapon(WeaponData data)
            {
                container.Weapons.Add(data);
            }

            void AddLocation(LocationData data)
            {
                container.Locations.Add(data);
            }

            void AddEnemy(EnemyData data)
            {
                container.Enemies.Add(data);
            }

            void AddProjectile(ProjectileData data)
            {
                model.Projectiles.Add(data.IdMeta, data);
            }
        }

        public static void CreateUserData(in GameMetaData meta, out UserData target)
        {
            var user = new UserData
            {
                Data = meta.Data.Copy()
            };

            // No need to store "user's enemies"
            user.Data.Enemies.Clear();

            user.Data.Player.IdCurrentWeapon = ID_PISTOL.GetIdUser();

            target = user;
        }

        public static void CreateGameplaySessionData(in UserData user, out GameplaySessionData target)
        {
            var session = new GameplaySessionData
            {
                Data = user.Data.Copy()
            };

            session.Data.Player.IdCurrentWeapon = user.Data.Player.IdCurrentWeapon.GetIdSession();
            session.IdCurrentLocation = user.Data.Locations.FirstOrDefault().IdSession;
            session.CurrentStage = 0;

            target = session;
        }

        public static void GetTestGameplaySessionData(out GameplaySessionData target)
        {
            CreateGameMetaData(out var meta);
            CreateUserData(in meta, out var user);
            CreateGameplaySessionData(in user, out var session);
            target = session;
        }
    }
}
