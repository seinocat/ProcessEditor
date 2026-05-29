using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Process.Runtime
{
    public static class ProcessGraphValidator
    {
        public static bool TryValidate(ProcessConfig config, out int startNodeOrder, out string error)
        {
            startNodeOrder = -1;
            error = string.Empty;

            if (config == null)
            {
                error = "Process config is null";
                return false;
            }

            var nodeDataList = config.NodeDataList;
            if (nodeDataList == null || nodeDataList.Count == 0)
            {
                error = $"Process config invalid, ProcessId: {config.ProcessId}, node list is empty";
                return false;
            }

            var errors = new List<string>();
            var orderSet = new HashSet<int>();
            var nodeOrderMap = new Dictionary<int, ProcessNodeData>();

            int startNodeCount = 0;
            int endNodeCount = 0;

            foreach (var nodeData in nodeDataList)
            {
                if (!orderSet.Add(nodeData.Order))
                {
                    errors.Add($"duplicate node order: {nodeData.Order}");
                    continue;
                }

                nodeOrderMap[nodeData.Order] = nodeData;

                if (nodeData.Type == ProcessNodeType.Start)
                {
                    startNodeCount++;
                    startNodeOrder = nodeData.Order;
                }

                if (nodeData.Type == ProcessNodeType.End)
                    endNodeCount++;
            }

            if (startNodeCount != 1)
                errors.Add($"start node count: {startNodeCount}, expected: 1");

            if (endNodeCount < 1)
                errors.Add($"end node count: {endNodeCount}, expected: >= 1");

            foreach (var nodeData in nodeDataList)
            {
                ValidateEdgeRefs(nodeData.NextNodeOrderList, nodeData.Order, "next", nodeOrderMap, errors);
                ValidateEdgeRefs(nodeData.SequenceNodeOrderList, nodeData.Order, "sequence", nodeOrderMap, errors);
            }

            if (errors.Count > 0)
            {
                error = BuildError(config.ProcessId, errors);
                return false;
            }

            if (!ValidateReachability(nodeOrderMap, startNodeOrder, out var reachabilityError))
            {
                error = $"Process config invalid, ProcessId: {config.ProcessId}, {reachabilityError}";
                return false;
            }

            return true;
        }

        private static void ValidateEdgeRefs(
            List<int> refs,
            int currentOrder,
            string edgeType,
            Dictionary<int, ProcessNodeData> nodeOrderMap,
            List<string> errors)
        {
            if (refs == null || refs.Count == 0)
                return;

            for (int i = 0; i < refs.Count; i++)
            {
                var targetOrder = refs[i];
                if (!nodeOrderMap.ContainsKey(targetOrder))
                {
                    errors.Add($"{edgeType} ref not found, node order: {currentOrder}, target order: {targetOrder}");
                }
            }
        }

        private static bool ValidateReachability(Dictionary<int, ProcessNodeData> nodeOrderMap, int startOrder, out string error)
        {
            error = string.Empty;
            if (!nodeOrderMap.ContainsKey(startOrder))
            {
                error = $"start node order not found: {startOrder}";
                return false;
            }

            var visited = new HashSet<int>();
            var queue = new Queue<int>();
            queue.Enqueue(startOrder);
            visited.Add(startOrder);

            while (queue.Count > 0)
            {
                var currentOrder = queue.Dequeue();
                var node = nodeOrderMap[currentOrder];

                EnqueueAll(node.NextNodeOrderList, visited, queue);
                EnqueueAll(node.SequenceNodeOrderList, visited, queue);
            }

            if (visited.Count == nodeOrderMap.Count)
                return true;

            var unreachable = nodeOrderMap.Keys.Where(order => !visited.Contains(order)).OrderBy(order => order);
            error = $"unreachable node orders: [{string.Join(", ", unreachable)}]";
            return false;
        }

        private static void EnqueueAll(List<int> refs, HashSet<int> visited, Queue<int> queue)
        {
            if (refs == null)
                return;

            for (int i = 0; i < refs.Count; i++)
            {
                var target = refs[i];
                if (visited.Add(target))
                    queue.Enqueue(target);
            }
        }

        private static string BuildError(ulong processId, List<string> errors)
        {
            var builder = new StringBuilder();
            builder.Append($"Process config invalid, ProcessId: {processId}");
            for (int i = 0; i < errors.Count; i++)
            {
                builder.Append(", ");
                builder.Append(errors[i]);
            }
            return builder.ToString();
        }
    }
}
