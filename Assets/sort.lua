-- [[
--     快速排序
-- ]]

local function swap(arr, i, j)
    arr[i] = arr[i] + arr[j]
    arr[j] = arr[i] - arr[j]
    arr[i] = arr[i] - arr[j]
end

local function partition(arr, left, right)
    local pivot = arr[math.floor((left+right)/2)]
    local i,j = left, right
    while i<j do
        while i<j and arr[i] < pivot do
            i = i + 1
        end
        while i<j and arr[j] > pivot do
            j = j - 1
        end
        if i<j then
            swap(arr, i, j)
        end
    end
    
    return i+1
end

local function quickSort(arr, left, right)
    if #arr < 2 then
        return
    end

    local index = partition(arr, left, right)
    if left < index -1 then
        quickSort(arr, left, index-1)
    end
    if index < right then
        quickSort(arr, index, right)
    end
end

local function printSort(arr)
    print(table.concat(arr, ","))
end

-- [[
-- 冒泡算法，两层遍历
-- ]]
local function bubbleSort(arr)
    local len = #arr
    for i=1,len-1 do
        for j=i+1,len do
            if arr[i]>arr[j] then
                swap(arr, i, j)
            end
        end
    end
end

-- [[
-- 选择排序，选出一个最小/大的Index
-- ]]
local function chooseSort(arr)
    local len = #arr
    for i=1, len-1 do
        local min = i
        for j=i+1, len do
            if arr[j] < arr[min] then
                min = j
            end
        end
        if i~=min then
            swap(arr, i, min)
        end
    end
end

-- [[
-- 插入排序，打扑克，从后面序列列表里面选择一个往前面有序的插
-- ]]
local function selectSort(arr)
    -- body
    local len,prevIndex,curr = #arr
    if len<2 then return end
    for i=2,len do
        prevIndex = i-1
        curr = arr[i]
        while prevIndex>0 and arr[prevIndex] > curr do
            arr[prevIndex+1] = arr[prevIndex]
            prevIndex = prevIndex-1
        end
        arr[prevIndex+1] = curr
    end
end

-- [[
--     归并排序，二分法，拆分到1个元素，比较，合并，返回
-- ]]
local function mergeSort(arr)
    local len = #arr
    if len<2 then
        return arr
    end
    local middle = math.floor(len/2)
    local left = tableSlice(arr, 1, middle)
    local right = tableSlice(arr, middle+1, len)
    return merge(mergeSort(left), mergeSort(right))
end

function tableSlice(t, s, e)
    return {unpack(t, s, e)}
end

function merge(left, right)
    local res = {}
    local i,j=1,1
    while i<=#left and j<=#right do
        if left[i] < right[j] then
            table.insert(res, left[i])
            i = i+1
        else
            table.insert(res, right[j])
            j = j+1
        end
    end
    if i<=#left then
        while i<=#left do
            table.insert(res, left[i])
            i = i+1
        end

    end
    if j<=#right then
        while j<=#right do
            table.insert(res, right[j])
            j = j+1
        end
    end

    return res
end

-- [[
--     堆排序，大顶堆/小顶堆，先建堆，取顶端元素到尾部，堆减一，堆化
-- ]]
local function heapSort(arr)
    buildMaxHeap(arr)

    local len = #arr
    for i=len, 2, -1 do
        swap(arr, 1, i)
        len = len-1
        heapify(arr, 1, len)
    end
end

function buildMaxHeap(arr)
    local len = #arr
    for i=math.floor(len/2), 1, -1 do
        heapify(arr, i, len)
    end
end

function heapify(arr, i, len)
    local left = i*2
    local right = i*2+1
    local largest = i

    if left<=len and arr[left]>arr[largest] then
        largest = left
    end
    if right<=len and arr[right]>arr[largest] then
        largest = right
    end
    if largest~=i then
        swap(arr, i, largest)
        heapify(arr, largest, len)
    end
end


local function test()
    local t = {5, 1, 3,2, 4}
    -- quickSort(t, 1, #t)
    -- bubbleSort(t)
    -- chooseSort(t)
    -- selectSort(t)
    -- local t = mergeSort(t)
    heapSort(t)
    printSort(t)
    
end

local function testTableSortAPI()
    -- local t = {5, 1, 1,2, 4}
    local t = {9,15,9,222,10}
    table.sort(t, function (a, b)
        print("compare:"..a.."-"..b)
        if a <= b then
            return true
        else
            return false
        end
    end)

    printSort(t)
end

test()
-- testTableSortAPI()

-- [[
--     A*算法 伪代码
--      贪婪算法，以最快的速度找出路径，不一定是最短路径
-- ]]

local function findPath(s, e)
    local open = {}
    local close = {}

    table.insert(open, s)
    table.insert(close, s)

    while #open>0 do
        local p = table.remove(open, 1)

        if p==e then break end

        for i,pp in ipairs(p.getNeighbor()) do --获取上下左右路点
            pp.f = pp.dis(e)
            if pp.isNotInOpen() then
                pp.setInopen()
                table.insert(open, pp)
            end
            
        end

        table.insert(close, p)

        table.sort(open, function (a, b)
            return a.f < b.f
        end)
    end
end

-- [[
--     A*算法优化
--     open队列的排序可以优化，使用最小堆数据结构优化
-- ]]
function Minheap()
    return {}
end
local function findPathWithMinheap(s, e)
    local open = Minheap()
    local close = {}

    table.insert(open, s)
    table.insert(close, s)

    while #open>0 do
        local p = table.remove(open, 1)

        if p==e then break end

        for i,pp in ipairs(p.getNeighbor()) do --获取上下左右路点
            pp.f = pp.dis(e)
            if pp.isNotInOpen() then
                pp.setInopen()
                open.add(pp) --加入后会自动堆化
            end
            
        end

        table.insert(close, p)
    end
end