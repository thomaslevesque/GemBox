using System;
using System.Configuration;
using System.Linq.Expressions;

namespace GemBox.Configuration
{
    public static class ConfigurationExtensions
    {
        public static void UpgradeIfRequired<TSettings>(
            this TSettings settings,
            string upgradeFlagName)
            where TSettings : ApplicationSettingsBase
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (upgradeFlagName == null) throw new ArgumentNullException("upgradeFlagName");
            bool flag = (bool)settings[upgradeFlagName];
            if (!flag)
            {
                settings.Upgrade();
                settings[upgradeFlagName] = true;
                settings.Save();
            }
        }

        public static void UpgradeIfRequired<TSettings>(
            this TSettings settings,
            Expression<Func<TSettings, bool>> upgradeFlagSelector)
            where TSettings : ApplicationSettingsBase
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (upgradeFlagSelector == null) throw new ArgumentNullException("upgradeFlagSelector");
            var upgradeFlagName = GetPropertyName(upgradeFlagSelector);
            settings.UpgradeIfRequired(upgradeFlagName);
        }

        private static string GetPropertyName<TInstance, TProperty>(Expression<Func<TInstance, TProperty>> expression)
        {
            var memberExpr = expression.Body as MemberExpression;
            if (memberExpr == null)
                throw new ArgumentException("Expression body is not a member access expression");
            return memberExpr.Member.Name;
        }
    }

}
