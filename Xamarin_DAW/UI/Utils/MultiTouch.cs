using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Xamarin_DAW.UI.Utils
{
    using static Skia_UI_Kit.NanoTime;

    public class MultiTouch
    {
        public enum TouchState
        {
            NONE,
            TOUCH_DOWN,
            TOUCH_MOVE,
            TOUCH_UP,
            TOUCH_CANCELLED
        };

        public class TouchData
        {
            public object identity;
            public long timestamp_nanoseconds;
            public long timestamp_nanoseconds_TOUCH_DOWN;
            public long timestamp_nanoseconds_TOUCH_MOVE;
            public long timestamp_nanoseconds_TOUCH_UP;
            public long timestamp_nanoseconds_TOUCH_CANCELLED;
            public float x;
            public float y;
            public float size;
            public float pressure;
            public TouchState state;
            public bool moved;
            internal float deltaDownLocationX;
            internal float deltaDownLocationY;
            internal float deltaLocationX;
            internal float deltaLocationY;
            public float deltaX;
            public float deltaY;

            internal void ComputeDelta(TouchData previous)
            {
                deltaDownLocationX = previous.deltaDownLocationX;
                deltaDownLocationY = previous.deltaDownLocationY;
                deltaX = deltaLocationX - deltaDownLocationX;
                deltaY = deltaLocationY - deltaDownLocationY;
                moved = previous.x != x || previous.y != y;
            }

            public PointF MouseToPointF() => new PointF(x, y);
            public PointF DeltaToPointF() => new PointF(deltaX, deltaY);

            public TouchData()
                : this(0, 0, 0, 0, 0, 0, TouchState.NONE) { }
            public TouchData(object identity, long timestamp_nanoseconds, float x, float y, float deltaX, float deltaY, TouchState state)
                : this(identity, timestamp_nanoseconds, x, y, deltaX, deltaY, 0, 0, TouchState.NONE) { }
            public TouchData(object identity, long timestamp_nanoseconds, float x, float y, float deltaX, float deltaY, float size, TouchState state)
                : this(identity, timestamp_nanoseconds, x, y, deltaX, deltaY, size, 0, TouchState.NONE) { }
            public TouchData(object identity, long timestamp_nanoseconds, float x, float y, float deltaX, float deltaY, float size, float pressure, TouchState state)
                : this(identity, timestamp_nanoseconds, x, y, deltaX, deltaY, size, pressure, state, false) { }
            public TouchData(object identity, long timestamp_nanoseconds, float x, float y, float deltaX, float deltaY, float size, float pressure, TouchState state, bool moved)
            {
                this.identity = identity;
                this.timestamp_nanoseconds = timestamp_nanoseconds;
                timestamp_nanoseconds_TOUCH_UP = 0;
                timestamp_nanoseconds_TOUCH_MOVE = 0;
                timestamp_nanoseconds_TOUCH_DOWN = 0;
                timestamp_nanoseconds_TOUCH_CANCELLED = 0;
                this.x = x;
                this.y = y;
                if (state == TouchState.TOUCH_DOWN)
                {
                    deltaDownLocationX = deltaX;
                    deltaDownLocationY = deltaY;
                }
                deltaLocationX = deltaX;
                deltaLocationY = deltaY;
                this.deltaX = 0;
                this.deltaY = 0;
                this.size = size;
                this.pressure = pressure;
                this.state = state;
                this.moved = moved;
            }
        };

        private class TouchContainer
        {
            public bool used;
            public TouchData touch;
            public TouchContainer()
            {
                used = false;
                touch = new TouchData();
            }
            public TouchContainer(bool used, TouchData touch)
            {
                this.used = used;
                this.touch = touch;
            }
        };

        List<TouchContainer> data = new();

        int maxSupportedTouches = 0;
        int touchCount = 0;
        int index;

        public bool debug = false;
        public bool printMoved = false;
        public bool throw_on_error = false;

        public TouchData getTouchAt(int index)
        {
            return data.ElementAt(index).touch;
        }

        public TouchData getTouchAtCurrentIndex()
        {
            return data.ElementAt(index).touch;
        }

        public int getTouchCount() {
            return touchCount;
        }

        public int getTouchIndex() {
            return index;
        }

        public class Iterator
        {
            MultiTouch multiTouch;
            int index = 0;
            public Iterator(MultiTouch multiTouch) {
                this.multiTouch = multiTouch;
            }
            public bool hasNext() {
                for (int i = index; i < multiTouch.maxSupportedTouches; i++)
                {
                    TouchContainer tc = multiTouch.data.ElementAt(i);
                    // a container can be marked as unused but have a touch state != NONE
                    // in this case, it is either freshly removed, or freshly cancelled
                    if (tc.touch.state != TouchState.NONE)
                    {
                        index = i;
                        return true;
                    }
                }
                return false;
            }
            public TouchData next() {
                return multiTouch.data.ElementAt(index++).touch;
            }
            public long getIndex() {
                return index-1;
            }
        };

        public Iterator getIterator() {
            return new Iterator(this);
        }

        private void listResize<T>(List<T> list, int size) where T : new()
        {
            if (size == 0)
            {
                list.Clear();
            }
            else
            {
                if (size > list.Count)
                {
                    while (list.Count < size)
                    {
                        list.Add(new T());
                    }
                }
                else if (size < list.Count)
                {
                    while (list.Count > size)
                    {
                        list.RemoveAt(list.Count - 1);
                    }
                }
            }
        }

        public void setMaxSupportedTouches(int supportedTouches) {
            maxSupportedTouches = supportedTouches;
            listResize(data, maxSupportedTouches);
        }

        public long getMaxSupportedTouches() {
            return maxSupportedTouches;
        }

        void tryPurgeTouch(TouchContainer touchContainer) {
            if (!touchContainer.used && touchContainer.touch.state != TouchState.NONE)
            {
                touchContainer.touch.moved = false;
                touchContainer.touch.state = TouchState.NONE;
                touchCount--;
            }
        }

        public void addTouch(TouchData touchData) {
            if (debug) Console.WriteLine("adding touch with identity: " + touchData.identity);
            bool found = false;
            for (int i = 0; i < maxSupportedTouches; i++)
            {
                TouchContainer touchContainer = data.ElementAt(i);
                tryPurgeTouch(touchContainer);
                if (!found && !touchContainer.used)
                {
                    found = true;
                    // when a touch is added all timestamps should be reset
                    touchContainer.touch = touchData;
                    touchContainer.touch.state = TouchState.TOUCH_DOWN;
                    touchContainer.touch.timestamp_nanoseconds_TOUCH_DOWN = touchData.timestamp_nanoseconds;
                    touchContainer.used = true;
                    touchCount++;
                    index = i;
                }
                if (touchContainer.used && touchContainer.touch.state == TouchState.NONE)
                {
                    if (throw_on_error)
                    {
                        throw new InvalidOperationException("touch cannot be active with a state of NONE");
                    }
                    else
                    {
                        Console.WriteLine("touch cannot be active with a state of NONE, cancelling touch");
                        cancelTouch();
                    }
                }
            }
            if (!found)
            {
                if (throw_on_error)
                {
                    throw new InvalidOperationException(
                            "the maximum number of supported touches of " +
                            maxSupportedTouches + " has been reached.\n" +
                            "please call setMaxSupportedTouches(long)"
                    );
                }
                else
                {
                    Console.WriteLine(
                            "the maximum number of supported touches of " +
                            maxSupportedTouches + " has been reached.\n" +
                            "please call setMaxSupportedTouches(long), cancelling touch"
                    );
                    cancelTouch();
                }
            }
        }

        public void addTouch(object identity, float x, float y, float deltaX, float deltaY) {
            addTouch(identity, x, y, deltaX, deltaY, 0, 0);
        }
        public void addTouch(object identity, float x, float y, float deltaX, float deltaY, float size) {
            addTouch(identity, x, y, deltaX, deltaY, size, 0);
        }
        public void addTouch(object identity, float x, float y, float deltaX, float deltaY, float size, float pressure) {
            addTouch(new TouchData(identity, currentTimeNanos(), x, y, deltaX, deltaY, size, pressure, TouchState.TOUCH_DOWN, false));
        }

        public void moveTouch(TouchData touchData) {
            if (debug && printMoved) Console.WriteLine("moving touch with identity: " + touchData.identity);
            bool found = false;
            for (int i = 0; i < maxSupportedTouches; i++)
            {
                TouchContainer touchContainer = data.ElementAt(i);
                tryPurgeTouch(touchContainer);
                if (!found && touchContainer.used)
                {
                    if (touchContainer.touch.identity == touchData.identity)
                    {
                        found = true;
                        TouchData previous = touchContainer.touch;
                        touchData.ComputeDelta(previous);
                        touchContainer.touch = touchData;
                        touchContainer.touch.timestamp_nanoseconds_TOUCH_DOWN = previous.timestamp_nanoseconds_TOUCH_DOWN;
                        touchContainer.touch.timestamp_nanoseconds_TOUCH_MOVE = touchData.timestamp_nanoseconds;
                        touchContainer.touch.timestamp_nanoseconds_TOUCH_UP = previous.timestamp_nanoseconds_TOUCH_UP;
                        touchContainer.touch.timestamp_nanoseconds_TOUCH_CANCELLED = previous.timestamp_nanoseconds_TOUCH_CANCELLED;
                        index = i;
                    }
                }
                if (touchContainer.used && touchContainer.touch.state == TouchState.NONE)
                {
                    if (throw_on_error)
                    {
                        throw new InvalidOperationException("touch cannot be active with a state of NONE");
                    }
                    else
                    {
                        Console.WriteLine("touch cannot be active with a state of NONE, cancelling touch");
                        cancelTouch();
                    }
                }
            }
            if (!found)
            {
                if (throw_on_error)
                {
                    throw new InvalidOperationException("cannot move a touch that has not been registered");
                }
                else
                {
                    Console.WriteLine("cannot move a touch that has not been registered, cancelling touch");
                    cancelTouch();
                }
            }
        }

        public void moveTouch(object identity, float x, float y, float deltaX, float deltaY)
        {
            moveTouch(identity, x, y, deltaX, deltaY, 0, 0);
        }
        public void moveTouch(object identity, float x, float y, float deltaX, float deltaY, float size)
        {
            moveTouch(identity, x, y, deltaX, deltaY, size, 0);
        }
        public void moveTouch(object identity, float x, float y, float deltaX, float deltaY, float size, float pressure)
        {
            moveTouch(new TouchData(identity, currentTimeNanos(), x, y, deltaX, deltaY, size, pressure, TouchState.TOUCH_MOVE));
        }

        public void removeTouch(TouchData touchData) {
            if (debug) Console.WriteLine("removing touch with identity: " + touchData.identity);
            bool found = false;
            for (int i = 0; i < maxSupportedTouches; i++)
            {
                TouchContainer touchContainer = data.ElementAt(i);
                tryPurgeTouch(touchContainer);
                if (!found && touchContainer.used)
                {
                    if (touchContainer.touch.identity == touchData.identity)
                    {
                        found = true;
                        TouchData previous = touchContainer.touch;
                        touchData.ComputeDelta(previous);
                        touchContainer.touch = touchData;
                        touchContainer.touch.timestamp_nanoseconds_TOUCH_DOWN = previous.timestamp_nanoseconds_TOUCH_DOWN;
                        touchContainer.touch.timestamp_nanoseconds_TOUCH_MOVE = previous.timestamp_nanoseconds_TOUCH_MOVE;
                        touchContainer.touch.timestamp_nanoseconds_TOUCH_UP = touchData.timestamp_nanoseconds;
                        touchContainer.touch.timestamp_nanoseconds_TOUCH_CANCELLED = previous.timestamp_nanoseconds_TOUCH_CANCELLED;
                        touchContainer.used = false;
                        index = i;
                    }
                }
                if (touchContainer.used && touchContainer.touch.state == TouchState.NONE)
                {
                    if (throw_on_error)
                    {
                        throw new InvalidOperationException("touch cannot be active with a state of NONE");
                    }
                    else
                    {
                        Console.WriteLine("touch cannot be active with a state of NONE, cancelling touch");
                        cancelTouch();
                    }
                }
            }
            if (!found)
            {
                if (throw_on_error)
                {
                    throw new InvalidOperationException("cannot remove a touch that has not been registered");
                }
                else
                {
                    Console.WriteLine("cannot remove a touch that has not been registered, cancelling touch");
                    cancelTouch();
                }
            }
        }

        public void removeTouch(object identity, float x, float y, float deltaX, float deltaY)
        {
            removeTouch(identity, x, y, deltaX, deltaY, 0, 0);
        }
        public void removeTouch(object identity, float x, float y, float deltaX, float deltaY, float size)
        {
            removeTouch(identity, x, y, deltaX, deltaY, size, 0);
        }
        public void removeTouch(object identity, float x, float y, float deltaX, float deltaY, float size, float pressure)
        {
            removeTouch(new TouchData(identity, currentTimeNanos(), x, y, deltaX, deltaY, size, pressure, TouchState.TOUCH_UP));
        }

        public void cancelTouch() {
            long timestamp = currentTimeNanos();
            // cancel the first touch
            if (maxSupportedTouches <= 0)
            {
                Console.WriteLine("the maximum number of supported touches must be greater than zero");
            }
            else
            {
                TouchContainer touchContainer = data.ElementAt(0);
                touchContainer.touch.moved = false;
                touchContainer.touch.state = TouchState.TOUCH_CANCELLED;
                touchContainer.touch.timestamp_nanoseconds = timestamp;
                touchContainer.touch.timestamp_nanoseconds_TOUCH_CANCELLED = timestamp;
                touchContainer.used = false;
            }
            index = 0;
        }

        public void cancelTouch(TouchData touchData) {
            if (debug) Console.WriteLine("cancelling touch");
            bool found = false;
            for (int i = 0; i < maxSupportedTouches; i++)
            {
                TouchContainer touchContainer = data[i];
                if (touchContainer.used && touchContainer.touch.state == TouchState.NONE)
                {
                    if (throw_on_error)
                    {
                        throw new InvalidOperationException("touch cannot be active with a state of NONE");
                    }
                    else
                    {
                        Console.WriteLine("touch cannot be active with a state of NONE, cancelling touch");
                        cancelTouch();
                    }
                }
                // ignore touch identity since we are cancelling a touch
                // the identity may not match at all
                if (touchContainer.used)
                {
                    if (!found)
                    {
                        found = true;
                        TouchData previous = touchContainer.touch;
                        touchData.ComputeDelta(previous);
                        touchContainer.touch = touchData;
                        touchContainer.touch.timestamp_nanoseconds_TOUCH_DOWN = previous.timestamp_nanoseconds_TOUCH_DOWN;
                        touchContainer.touch.timestamp_nanoseconds_TOUCH_MOVE = previous.timestamp_nanoseconds_TOUCH_MOVE;
                        touchContainer.touch.timestamp_nanoseconds_TOUCH_UP = previous.timestamp_nanoseconds_TOUCH_UP;
                        touchContainer.touch.timestamp_nanoseconds_TOUCH_CANCELLED = touchData.timestamp_nanoseconds;
                        touchContainer.used = false;
                        index = i;
                    }
                    else
                    {
                        touchContainer.touch.state = TouchState.NONE;
                        touchContainer.used = false;
                    }
                }
            }

            if (!found)
            {
                // if not found, cancel the first touch
                if (maxSupportedTouches <= 0)
                {
                    Console.WriteLine("the maximum number of supported touches must be greater than zero");
                }
                else
                {
                    TouchContainer touchContainer = data[0];
                    TouchData previous = touchContainer.touch;
                    touchData.ComputeDelta(previous);
                    touchContainer.touch = touchData;
                    touchContainer.touch.timestamp_nanoseconds_TOUCH_DOWN = previous.timestamp_nanoseconds_TOUCH_DOWN;
                    touchContainer.touch.timestamp_nanoseconds_TOUCH_MOVE = previous.timestamp_nanoseconds_TOUCH_MOVE;
                    touchContainer.touch.timestamp_nanoseconds_TOUCH_UP = previous.timestamp_nanoseconds_TOUCH_UP;
                    touchContainer.touch.timestamp_nanoseconds_TOUCH_CANCELLED = touchData.timestamp_nanoseconds;
                    touchContainer.touch.state = TouchState.TOUCH_CANCELLED;
                    touchContainer.used = false;
                }
                index = 0;
            }
            touchCount = 0;
        }

        public void cancelTouch(object identity, float x, float y, float deltaX, float deltaY)
        {
            cancelTouch(identity, x, y, deltaX, deltaY, 0, 0);
        }
        public void cancelTouch(object identity, float x, float y, float deltaX, float deltaY, float size)
        {
            cancelTouch(identity, x, y, deltaX, deltaY, size, 0);
        }
        public void cancelTouch(object identity, float x, float y, float deltaX, float deltaY, float size, float pressure)
        {
            cancelTouch(new TouchData(identity, currentTimeNanos(), x, y, deltaX, deltaY, size, pressure, TouchState.TOUCH_CANCELLED));
        }

        public static string stateToString(TouchState state) {
            switch (state)
            {
                case TouchState.NONE: return "NONE";
                case TouchState.TOUCH_DOWN: return "TOUCH_DOWN";
                case TouchState.TOUCH_MOVE: return "TOUCH_MOVE";
                case TouchState.TOUCH_UP: return "TOUCH_UP";
                default: return "TOUCH_CANCELLED";
            }
        }

        public override string ToString()
        {
            string s = "";
            s += "touch count : " + touchCount;
            for (int touchIndex = 0; touchIndex < maxSupportedTouches; touchIndex++)
            {
                TouchContainer touchContainer = data.ElementAt(touchIndex);
                TouchData touch = touchContainer.touch;
                if (touch.state != TouchState.NONE)
                {
                    s += "\n touch index : " + touchIndex;
                    if (touchIndex == index) s += " [CURRENT]";
                    s += ", action : " + stateToString(touch.state);
                    s += ", identity : " + touch.identity;
                    s += ", timestamp (nanoseconds) : " + touch.timestamp_nanoseconds;
                    s += ", timestamp (TOUCH_DOWN nanoseconds) : " + touch.timestamp_nanoseconds_TOUCH_DOWN;
                    s += ", timestamp (TOUCH_MOVE nanoseconds) : " + touch.timestamp_nanoseconds_TOUCH_MOVE;
                    s += ", timestamp (TOUCH_UP nanoseconds) : " + touch.timestamp_nanoseconds_TOUCH_UP;
                    s += ", timestamp (TOUCH_CANCELLED nanoseconds) : " + touch.timestamp_nanoseconds_TOUCH_CANCELLED;
                    s += ", did touch move : " + (touch.moved ? "True" : "False");
                    s += ", x : " + touch.x;
                    s += ", y : " + touch.y;
                    s += ", delta down location x : " + touch.deltaDownLocationX;
                    s += ", delta down location y : " + touch.deltaDownLocationY;
                    s += ", delta location x : " + touch.deltaLocationX;
                    s += ", delta location y : " + touch.deltaLocationY;
                    s += ", delta x : " + touch.deltaX;
                    s += ", delta y : " + touch.deltaY;
                    s += ", size : " + touch.size;
                    s += ", pressure : " + touch.pressure;
                }
            }
            return s;
        }
    }
}
