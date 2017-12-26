

public class Prefab {
    private static Prefab instance = new Prefab();
    private Prefab() { }
    public static Prefab getInstance() {
        return instance;
    }

    // 具有碰撞trigger属性的物体
    public string[] triggerPath = {
        "Waterfall",
        "Decorations",
        "DieRegion",
        "Waterfall1",
        "Decorations1",
    };

    // 需要播放帧动画的物体
    public string[] animationPath = {
        "Waterfall",
        "Waterfall1",
    };

    // 具有上升平台属性的物体
    public string[] platformPath = {
        "Waterfall",
        "Waterfall1",
    };

    // 具有梯子(可攀爬)属性的物体
    public string[] ladderPath = {
        "Decorations",
        "Decorations1",
    };
}
