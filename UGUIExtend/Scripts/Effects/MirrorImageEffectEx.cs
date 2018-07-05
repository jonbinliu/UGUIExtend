//出处：https://github.com/L-Lawliet/UGUIExtend/blob/master/Scripts/UGUIExtend/Effects/Mirror.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Sprites;

public class MirrorImageEffect : BaseVertexEffect
{
    public enum MirrorType
    {
        /// <summary>
        /// 水平 left -> right
        /// </summary>
        HorizontalLR,

        /// <summary>
        /// 垂直 bottom -> top 
        /// </summary>
        VerticalBT,

        /// <summary>
        /// 水平 right -> left
        /// </summary>
        HorizontalRL,

        /// <summary>
        /// 垂直 top -> bottom 
        /// </summary>
        VerticalTB,

        /// <summary>
        /// 四分之一
        /// //先左往右翻转，从下至上翻转  left -> right -> bottom -> top 
        /// </summary>
        QuarterLRBT,

        /// <summary>
        /// 四分之一
        /// //先左往右翻转，从上至下翻转  left -> right -> top -> bottom 
        /// </summary>
        QuarterLRTB,

        /// <summary>
        /// 四分之一
        /// //先左往右翻转，从下至上翻转  right -> left -> bottom -> top 
        /// </summary>
        QuarterRLBT,

        /// <summary>
        /// 四分之一
        /// //先往右翻转，从上至下翻转  right -> left -> top -> left
        /// </summary>
        QuarterRLTB,
    }

    public enum MirrorDirectionType
    {
        Left2Right,
        Right2Left,
        Bottom2Top,
        Top2Bottom,
    }

    [SerializeField]
    private MirrorType m_MirrorType = MirrorType.HorizontalLR;
    public MirrorType mirrorType
    {
        get { return m_MirrorType; }
        set
        {
            if (m_MirrorType != value)
            {
                m_MirrorType = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }
    }

    private Image bindImage
    {
        get
        {
            return gameObject.GetComponent<Image>();
        }
    }

    private Sprite bindSprite
    {
        get
        {
            Sprite _bindSprite = null;

            if (bindImage != null)
            {
                _bindSprite = bindImage.sprite;
            }

            return _bindSprite;
        }
    }

    private RectTransform bindRectTran
    {
        get
        {
            return gameObject.GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// 设置原始尺寸
    /// </summary>
    [ContextMenu("Set Native Size")]
    public void SetNativeSize()
    {
        if (graphic != null && graphic is Image)
        {
            Sprite overrideSprite = (graphic as Image).overrideSprite;

            if (overrideSprite != null)
            {
                float w = overrideSprite.rect.width / (graphic as Image).pixelsPerUnit;
                float h = overrideSprite.rect.height / (graphic as Image).pixelsPerUnit;

                //当anchorMax == anchorMin的时候。sizeDelta（x,y）与Rect的宽高是一致的。RectTransform与锚点偏移量就是本身的大小
                //这个时候可以通过设置sizeDelta来设置宽高
                bindRectTran.anchorMax = bindRectTran.anchorMin;

                switch (m_MirrorType)
                {
                    case MirrorType.HorizontalLR:
                    case MirrorType.HorizontalRL:
                        bindRectTran.sizeDelta = new Vector2(w * 2, h);
                        break;
                    case MirrorType.VerticalBT:
                    case MirrorType.VerticalTB:
                        bindRectTran.sizeDelta = new Vector2(w, h * 2);
                        break;
                    case MirrorType.QuarterLRBT:
                    case MirrorType.QuarterLRTB:
                    case MirrorType.QuarterRLBT:
                    case MirrorType.QuarterRLTB:
                        bindRectTran.sizeDelta = new Vector2(w * 2, h * 2);
                        break;
                }

                graphic.SetVerticesDirty();
            }
        }
    }

    /// <summary>
    /// 绘制Simple版
    /// </summary>
    /// <param name="output"></param>
    /// <param name="count"></param>
    protected void DrawSimple(List<UIVertex> output, int count)
    {
        Rect rect = graphic.GetPixelAdjustedRect();

        SimpleScaleEx(rect, output, count);

        switch (m_MirrorType)
        {
            case MirrorType.HorizontalLR:
                ExtendCapacity(output, count);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Left2Right);
                break;
            case MirrorType.HorizontalRL:
                ExtendCapacity(output, count);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Right2Left);
                break;
            case MirrorType.VerticalBT:
                ExtendCapacity(output, count);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Bottom2Top);
                break;
            case MirrorType.VerticalTB:
                ExtendCapacity(output, count);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Top2Bottom);
                break;
            case MirrorType.QuarterLRBT:
                ExtendCapacity(output, count * 3);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Left2Right);
                MirrorVertsEx(rect, output, count * 2, MirrorDirectionType.Bottom2Top);
                break;
            case MirrorType.QuarterLRTB:
                ExtendCapacity(output, count * 3);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Left2Right);
                MirrorVertsEx(rect, output, count * 2, MirrorDirectionType.Top2Bottom);
                break;
            case MirrorType.QuarterRLTB:
                ExtendCapacity(output, count * 3);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Right2Left);
                MirrorVertsEx(rect, output, count * 2, MirrorDirectionType.Top2Bottom);
                break;
            case MirrorType.QuarterRLBT:
                ExtendCapacity(output, count * 3);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Right2Left);
                MirrorVertsEx(rect, output, count * 2, MirrorDirectionType.Bottom2Top);

                break;
        }
    }

    /// <summary>
    /// 绘制Sliced版
    /// </summary>
    /// <param name="output"></param>
    /// <param name="count"></param>
    protected void DrawSliced(List<UIVertex> output, int count)
    {
        if (!(graphic as Image).hasBorder)
        {
            DrawSimple(output, count);
        }

        Rect rect = graphic.GetPixelAdjustedRect();

        SlicedScaleEx(rect, output, count);
        count = SliceExcludeVerts(output, count);

        switch (m_MirrorType)
        {
            case MirrorType.HorizontalLR:
                ExtendCapacity(output, count);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Left2Right);
                break;
            case MirrorType.HorizontalRL:
                ExtendCapacity(output, count);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Right2Left);
                break;
            case MirrorType.VerticalBT:
                ExtendCapacity(output, count);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Bottom2Top);
                break;
            case MirrorType.VerticalTB:
                ExtendCapacity(output, count);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Top2Bottom);
                break;
            case MirrorType.QuarterLRBT:
                ExtendCapacity(output, count * 3);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Left2Right);
                MirrorVertsEx(rect, output, count * 2, MirrorDirectionType.Bottom2Top);
                break;

            case MirrorType.QuarterLRTB:
                ExtendCapacity(output, count * 3);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Left2Right);
                MirrorVertsEx(rect, output, count * 2, MirrorDirectionType.Top2Bottom);
                break;
            case MirrorType.QuarterRLTB:
                ExtendCapacity(output, count * 3);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Right2Left);
                MirrorVertsEx(rect, output, count * 2, MirrorDirectionType.Top2Bottom);
                break;
            case MirrorType.QuarterRLBT:
                ExtendCapacity(output, count * 3);
                MirrorVertsEx(rect, output, count, MirrorDirectionType.Right2Left);
                MirrorVertsEx(rect, output, count * 2, MirrorDirectionType.Bottom2Top);

                break;
        }
    }

    /// <summary>
    /// 绘制Tiled版
    /// </summary>
    /// <param name="output"></param>
    /// <param name="count"></param>
    protected void DrawTiled(List<UIVertex> verts, int count)
    {
        Sprite overrideSprite = (graphic as Image).overrideSprite;

        if (overrideSprite == null)
        {
            return;
        }

        Rect rect = graphic.GetPixelAdjustedRect();

        //此处使用inner是因为Image绘制Tiled时，会把透明区域也绘制了。

        Vector4 inner = DataUtility.GetInnerUV(overrideSprite);

        float w = overrideSprite.rect.width / (graphic as Image).pixelsPerUnit;
        float h = overrideSprite.rect.height / (graphic as Image).pixelsPerUnit;

        int len = count / 3;

        for (int i = 0; i < len; i++)
        {
            UIVertex v1 = verts[i * 3];
            UIVertex v2 = verts[i * 3 + 1];
            UIVertex v3 = verts[i * 3 + 2];

            float centerX = GetCenter(v1.position.x, v2.position.x, v3.position.x);

            float centerY = GetCenter(v1.position.y, v2.position.y, v3.position.y);

            if (m_MirrorType == MirrorType.HorizontalLR
                || m_MirrorType == MirrorType.QuarterLRBT
                 || m_MirrorType == MirrorType.QuarterLRTB)
            {
                //判断三个点的水平位置是否在偶数矩形内，如果是，则把UV坐标水平翻转
                if (Mathf.FloorToInt((centerX - rect.xMin) / w) % 2 == 1)
                {
                    v1.uv0 = GetOverturnUV(v1.uv0, inner.x, inner.z, true);
                    v2.uv0 = GetOverturnUV(v2.uv0, inner.x, inner.z, true);
                    v3.uv0 = GetOverturnUV(v3.uv0, inner.x, inner.z, true);
                }
            }

            if (m_MirrorType == MirrorType.VerticalBT
                || m_MirrorType == MirrorType.QuarterRLBT
                || m_MirrorType == MirrorType.QuarterLRBT
                )
            {
                //判断三个点的垂直位置是否在偶数矩形内，如果是，则把UV坐标垂直翻转
                if (Mathf.FloorToInt((centerY - rect.yMin) / h) % 2 == 1)
                {
                    v1.uv0 = GetOverturnUV(v1.uv0, inner.y, inner.w, false);
                    v2.uv0 = GetOverturnUV(v2.uv0, inner.y, inner.w, false);
                    v3.uv0 = GetOverturnUV(v3.uv0, inner.y, inner.w, false);
                }
            }

            verts[i * 3] = v1;
            verts[i * 3 + 1] = v2;
            verts[i * 3 + 2] = v3;
        }
    }

    /// <summary>
    /// 返回翻转UV坐标
    /// </summary>
    /// <param name="uv"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <param name="isHorizontal"></param>
    /// <returns></returns>
    protected Vector2 GetOverturnUV(Vector2 uv, float start, float end, bool isHorizontal = true)
    {
        if (isHorizontal)
        {
            uv.x = end - uv.x + start;
        }
        else
        {
            uv.y = end - uv.y + start;
        }

        return uv;
    }

    /// <summary>
    /// 返回三个点的中心点
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <returns></returns>
    protected float GetCenter(float p1, float p2, float p3)
    {
        float max = Mathf.Max(Mathf.Max(p1, p2), p3);

        float min = Mathf.Min(Mathf.Min(p1, p2), p3);

        return (max + min) / 2;
    }

    /// <summary>
    /// 扩展容量
    /// </summary>
    /// <param name="verts"></param>
    /// <param name="addCount"></param>
    protected void ExtendCapacity(List<UIVertex> verts, int addCount)
    {
        var neededCapacity = verts.Count + addCount;

        if (verts.Capacity < neededCapacity)
        {
            verts.Capacity = neededCapacity;
        }
    }

    /// <summary>
    /// Simple缩放位移顶点（减半）
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="verts"></param>
    /// <param name="count"></param>
    protected void SimpleScaleEx(Rect rect, List<UIVertex> verts, int count)
    {
        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = verts[i];

            Vector3 position = vertex.position;

            //rect.x，rect.y是矩形左下角的点
            if (m_MirrorType == MirrorType.HorizontalLR
                || m_MirrorType == MirrorType.QuarterLRBT
                || m_MirrorType == MirrorType.QuarterLRTB
                )
            {
                //这里的意思，其实是相当于将rect.x当作是参考点，position.x 为距离，除0.5可以变回原来的大小
                position.x = (position.x + rect.x) * 0.5f;
            }
            else if (m_MirrorType == MirrorType.HorizontalRL
                || m_MirrorType == MirrorType.QuarterRLBT
                || m_MirrorType == MirrorType.QuarterRLTB
                )
            {
                //以矩形最右点为参考点,算出poistion.x原本应该处于的位置
                position.x = (rect.x + rect.width + position.x) * 0.5f;
            }

            if (m_MirrorType == MirrorType.VerticalBT
                || m_MirrorType == MirrorType.QuarterLRBT
                || m_MirrorType == MirrorType.QuarterRLBT
                )
            {
                position.y = (position.y + rect.y) * 0.5f;
            }
            else if (m_MirrorType == MirrorType.VerticalTB
                || m_MirrorType == MirrorType.QuarterLRTB
                || m_MirrorType == MirrorType.QuarterRLTB)
            {
                position.y = (rect.y + rect.height + position.y) * 0.5f;
            }

            vertex.position = position;

            verts[i] = vertex;
        }
    }

    /// <summary>
    /// Sliced缩放位移顶点（减半）
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="verts"></param>
    /// <param name="count"></param>
    protected void SlicedScaleEx(Rect rect, List<UIVertex> verts, int count)
    {
        Vector4 border = GetAdjustedBorders(rect);

        float xMax = rect.xMax;
        float xMin = rect.xMin;
        float yMax = rect.yMax;
        float yMin = rect.yMin;

        float xMaxHalf = rect.center.x;
        float yMaxHalf = rect.center.y;

        //算出border矩形的四个点
        Vector4 borderPos = new Vector4(
            xMin + border.x,
            yMax - border.y,
            xMax - border.z,
            yMin + border.w
            );

        //原作者的这个算法我没看懂。。。。先注释掉，按我自己的算法来
        //float halfWidth = rect.width * 0.5f;
        //float halfHeight = rect.height * 0.5f;

        //for (int i = 0; i < count; i++)
        //{
        //    UIVertex vertex = verts[i];

        //    Vector3 position = vertex.position;

        //    if (m_MirrorType == MirrorType.Horizontal || m_MirrorType == MirrorType.Quarter)
        //    {
        //        if (halfWidth < border.x && position.x >= rect.center.x)
        //        {
        //            position.x = rect.center.x;
        //        }
        //        else if (position.x >= border.x)
        //        {
        //            position.x = (position.x + rect.x) * 0.5f;
        //        }
        //    }

        //    if (m_MirrorType == MirrorType.Vertical || m_MirrorType == MirrorType.Quarter)
        //    {
        //        if (halfHeight < border.y && position.y >= rect.center.y)
        //        {
        //            position.y = rect.center.y;
        //        }
        //        else if (position.y >= border.y)
        //        {
        //            position.y = (position.y + rect.y) * 0.5f;
        //        }
        //    }

        //    vertex.position = position;

        //    verts[i] = vertex;
        //}

        //这里的想法就是除了border保持与两边的距离一致外，其余顶点按扩大后缩放一半
        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = verts[i];

            Vector3 position = vertex.position;

            if (m_MirrorType == MirrorType.HorizontalLR
                || m_MirrorType == MirrorType.QuarterLRBT
                || m_MirrorType == MirrorType.QuarterLRTB
                )
            {
                if (position.x == borderPos.z) //右边border顶点保持xMax固定的距离
                {
                    position.x = xMaxHalf - border.z;    //右边Border以Center为参考点
                }
                else if (position.x == borderPos.x)  //左边broder顶点保持与xMax固定的距离
                {
                    position.x = xMin + border.x;       //左边Border以
                }
                else  //其他顶点，缩放至原来的一半
                {
                    position.x = (position.x + rect.x) * 0.5f;
                }
            }
            else if (
                m_MirrorType == MirrorType.HorizontalRL
                || m_MirrorType == MirrorType.QuarterRLBT
                || m_MirrorType == MirrorType.QuarterRLTB
                )
            {
                if (position.x == borderPos.z) //右边border顶点保持xMax固定的距离
                {
                    position.x = xMax - border.z;   //以xMax为参考点来固定border的位置
                }
                else if (position.x == borderPos.x)  //左边broder顶点保持与xMax固定的距离
                {
                    position.x = xMaxHalf + border.x; //以Center为参考点
                }
                else  //其他顶点，缩放至原来的一半
                {
                    position.x = (rect.x + rect.width + position.x) * 0.5f;
                }
            }

            if (m_MirrorType == MirrorType.VerticalBT
                || m_MirrorType == MirrorType.QuarterLRBT
                || m_MirrorType == MirrorType.QuarterRLBT
                )
            {
                if (position.y == borderPos.y)
                {
                    position.y = yMaxHalf - border.y;
                }
                else if (position.y == borderPos.w)
                {
                    position.y = yMin + border.w;
                }
                else if (position.y >= border.y)
                {
                    position.y = (position.y + rect.y) * 0.5f;
                }
            }
            else if (
                m_MirrorType == MirrorType.VerticalTB
                || m_MirrorType == MirrorType.QuarterLRTB
                || m_MirrorType == MirrorType.QuarterRLTB
                )
            {
                if (position.y == borderPos.y)
                {
                    position.y = yMin - border.y;
                }
                else if (position.y == borderPos.w)
                {
                    position.y = yMaxHalf + border.w;
                }
                else if (position.y < border.y)
                {
                    position.y = (rect.y + rect.height + position.y) * 0.5f;
                }
            }

            vertex.position = position;

            verts[i] = vertex;
        }
    }

    /// <summary>
    /// 镜像顶点
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="verts"></param>
    /// <param name="count"></param>
    /// <param name="isHorizontal"></param>
    protected void MirrorVertsEx(Rect rect, List<UIVertex> verts, int count, MirrorDirectionType dirtectionType = MirrorDirectionType.Left2Right)
    {
        //以count倒序镜像，保持顶点顺序正确，以便在wireframe模式下可以看到正确的mesh
        for (int i = count - 1; i >= 0; i--)
        {
            UIVertex vertex = verts[i];

            Vector3 position = vertex.position;

            switch (dirtectionType)
            {
                case MirrorDirectionType.Left2Right:
                case MirrorDirectionType.Right2Left:
                    {
                        position.x = rect.center.x * 2 - position.x;
                        break;
                    }

                case MirrorDirectionType.Bottom2Top:
                case MirrorDirectionType.Top2Bottom:
                    {
                        position.y = rect.center.y * 2 - position.y;
                        break;
                    }
            }

            vertex.position = position;
            verts.Add(vertex);
        }
    }

    /// <summary>
    /// 清理掉不能成三角面的顶点
    /// </summary>
    /// <param name="verts"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    protected int SliceExcludeVerts(List<UIVertex> verts, int count)
    {
        int realCount = count;
        int i = 0;

        while (i < realCount)
        {
            UIVertex v1 = verts[i];
            UIVertex v2 = verts[i + 1];
            UIVertex v3 = verts[i + 2];
            UIVertex v4 = verts[i + 3];

            if (v1.position == v2.position || v2.position == v3.position || v3.position == v4.position || v4.position == v1.position)
            {
                verts[i] = verts[realCount - 4];
                verts[i + 1] = verts[realCount - 3];
                verts[i + 2] = verts[realCount - 2];
                verts[i + 3] = verts[realCount - 1];

                realCount -= 4;
                continue;
            }

            i += 4;
        }

        if (realCount < count)
        {
            verts.RemoveRange(realCount, count - realCount);
        }

        return realCount;
    }

    /// <summary>
    /// 返回矫正过的范围
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    protected Vector4 GetAdjustedBorders(Rect rect)
    {
        Sprite overrideSprite = (graphic as Image).overrideSprite;

        Vector4 border = overrideSprite.border;

        border = border / (graphic as Image).pixelsPerUnit;

        for (int axis = 0; axis <= 1; axis++)
        {
            float combinedBorders = border[axis] + border[axis + 2];

            if (rect.size[axis] < combinedBorders && combinedBorders != 0)
            {
                float borderScaleRatio = rect.size[axis] / combinedBorders;
                border[axis] *= borderScaleRatio;
                border[axis + 2] *= borderScaleRatio;
            }
        }
        return border;
    }

    public override void ModifyVertices(List<UIVertex> verts)
    {
        if (!IsActive())
        {
            return;
        }

        if (verts == null
            || verts.Count == 0)
        {
            Debug.LogWarning("No verts");
            return;
        }

        if (graphic is Image)
        {
            Image.Type type = (graphic as Image).type;

            switch (type)
            {
                case Image.Type.Simple:
                    DrawSimple(verts, verts.Count);
                    break;
                case Image.Type.Sliced:
                    DrawSliced(verts, verts.Count);
                    break;
                case Image.Type.Tiled:
                    DrawNone();
                    break;
                case Image.Type.Filled:
                    DrawNone();
                    break;
            }
        }
    }

    private void DrawNone()
    {
        Debug.LogWarning("No Implement!");
    }

}  //class