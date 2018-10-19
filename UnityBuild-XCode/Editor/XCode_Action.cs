using SuperSystems.UnityBuild.Interfaces;
using UnityEditor;

namespace SuperSystems.UnityBuild
{
    public abstract class XCode_Action : BuildAction, IPostProcessPerPlatformAction
    {
        private void OnEnable()
        {
            BuildFilter.FilterClause clause = new BuildFilter.FilterClause();
            clause.comparison = BuildFilter.FilterComparison.Equals;
            clause.type = BuildFilter.FilterType.Platform;
            clause.test = "iOS";

            filter = new BuildFilter();
            filter.condition = BuildFilter.FilterCondition.ExactlyOne;
            filter.clauses = new[] {clause};
        }


        public override void PostProcessExecute(BuildTarget buildTarget, string buildPath)
        {
#if UNITY_IOS
            Process(buildTarget,buildPath);
#endif
        }

        protected abstract void Process(BuildTarget buildTarget, string buildPath);
    }
}